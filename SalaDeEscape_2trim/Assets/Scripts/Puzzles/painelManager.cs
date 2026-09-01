using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class painelManager : MonoBehaviour
{
    public static painelManager instance;

    [Header("Estado do puzzle")]
    public bool puzzleAtivo = false;

    [Header("Configuracoes de cores")]
    [SerializeField] private int corSelecionada = -1;
    
    [Tooltip("Arrastar os materiais na ordem exata")]
    [SerializeField] private Material[] materiaisCores;

    [Header("Objeto do cenario")]
    [SerializeField] private GameObject fiosBloqueio;
    
    [Header("Camera")]
    [SerializeField] private FirstPersonLook look;
    private Camera mainCam;

    private int[,] matrizAtual;

    [Header("Carta de Dica")]
    [SerializeField] private Item cartaItem;
    [SerializeField] private GameObject cartaNaParede;
    [SerializeField] private PlayerInventory inventoryPlayer;

    [Header("Audio")]
    [SerializeField] private EventReference somBotao;
    [SerializeField] private EventReference somBolinha;

    [Header("Input System Settings")]
    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction acaoInteractMouse;
    private InputAction acaoSairPuzzle;

    // Gabarito da Bandeira (8 faixas de cores)
    private int[,] matrizGabarito = new int[,] {
        { 0, 0, 0, 0, 0, 0, 0, 0 }, // bola 1: Branco (0)
        { 0, 1, 1, 1, 1, 1, 1, 0 }, // bola 2: Vermelho (1)
        { 0, 2, 2, 2, 2, 2, 2, 0 }, // bola 3: Laranja (2)
        { 0, 4, 4, 4, 4, 4, 4, 0 }, // bola 4: Amarelo (4)
        { 0, 6, 6, 6, 6, 6, 6, 0 }, // bola 5: Verde (6)
        { 0, 3, 3, 3, 3, 3, 3, 0 }, // bola 6: Azul (3)
        { 0, 5, 5, 5, 5, 5, 5, 0 }, // bola 7: Roxo (5)
        { 0, 0, 0, 0, 0, 0, 0, 0 }  // bola 8: Branco (0)
    };

    private int totalLinhas;
    private int totalColunas;

    void Awake()
    {
        instance = this; 

        totalLinhas = matrizGabarito.GetLength(0);
        totalColunas = matrizGabarito.GetLength(1);
        matrizAtual = new int[totalLinhas, totalColunas];

        // Carrega ações do Input System sem dependência de hardware direto
        if (inputActionsAsset != null)
        {
            acaoInteractMouse = inputActionsAsset.FindAction("InteractMouse");
            acaoSairPuzzle = inputActionsAsset.FindAction("SairPuzzle");
        }
        else
        {
            PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
            if (playerInput != null)
            {
                acaoInteractMouse = playerInput.actions.FindAction("InteractMouse");
                acaoSairPuzzle = playerInput.actions.FindAction("SairPuzzle");
            }
        }
    }

    void Start()
    {
        mainCam = Camera.main;
        ResetarMatrizLimpa();
    }

    void ResetarMatrizLimpa()
    {
        for (int l = 0; l < totalLinhas; l++)
        {
            for (int c = 0; c < totalColunas; c++)
            {
                matrizAtual[l, c] = -1;
            }
        }
    }

    public void AbrirPuzzle()
    {
        puzzleAtivo = true;
        if (look != null) look.enabled = false;
        
        if (GerentUI.instance != null) 
            GerentUI.instance.SetPawCursor(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cartaNaParede != null && cartaItem != null && inventoryPlayer != null && inventoryPlayer.Itens.Contains(cartaItem))
        {
            cartaNaParede.SetActive(true);
        }
    }

    public void FecharPuzzle()
    {
        puzzleAtivo = false;
        if (look != null) look.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (!puzzleAtivo) return;

        // Ação de Sair via Input System
        if (acaoSairPuzzle != null && acaoSairPuzzle.WasPressedThisFrame())
        {
            FecharPuzzle();
            return;
        }

        // Ação de Interagir via Input System (Touch, Mouse ou Gamepad)
        if (acaoInteractMouse != null && acaoInteractMouse.WasPressedThisFrame())
        {
            ProcessarCliqueOuToque();
        }
    }

    void ProcessarCliqueOuToque()
    {
        Vector2 screenPosition = Vector2.zero;

        // Suporte Nativo Multiplataforma (Touch / Pointer)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Pointer.current != null)
        {
            screenPosition = Pointer.current.position.ReadValue();
        }

        Ray ray = mainCam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject objClicado = hit.collider.gameObject;

            // 1. BOTAO DE COR
            BotaoCor botao = objClicado.GetComponent<BotaoCor>();
            if (botao != null)
            {
                SelecionarCor(botao.CorId);
                if (!somBotao.IsNull) RuntimeManager.PlayOneShot(somBotao, hit.point);
                return;
            }

            // 2. BOLINHA
            if (objClicado.name.ToLower().StartsWith("bola") || objClicado.CompareTag("BolinhaPainel") || objClicado.name.StartsWith("Sphere"))
            {
                if (corSelecionada == -1)
                {
                    Debug.Log("Selecione uma cor primeiro!");
                    return;
                }

                int linha = 0;
                int coluna = 0;

                string[] partes = objClicado.name.Split(' ');

                if (partes.Length >= 2)
                {
                    int.TryParse(partes[1], out linha);
                    linha = Mathf.Max(0, linha - 1);
                }

                if (partes.Length >= 3)
                {
                    int.TryParse(partes[2], out coluna);
                    coluna = Mathf.Max(0, coluna - 1);
                }

                PintarBolinha(objClicado, linha, coluna);
            }
        }
    }

    public void SelecionarCor(int idCor)
    {
        corSelecionada = idCor;
        Debug.Log("Cor ativa alterada para ID: " + idCor);
    }

    void PintarBolinha(GameObject objetoBolinha, int linha, int coluna)
    {
        linha = Mathf.Clamp(linha, 0, totalLinhas - 1);
        coluna = Mathf.Clamp(coluna, 0, totalColunas - 1);

        matrizAtual[linha, coluna] = corSelecionada;

        MeshRenderer renderer = objetoBolinha.GetComponent<MeshRenderer>();
        if (renderer != null && corSelecionada >= 0 && corSelecionada < materiaisCores.Length)
        {
            renderer.material = materiaisCores[corSelecionada];
            if (!somBolinha.IsNull) RuntimeManager.PlayOneShot(somBolinha, objetoBolinha.transform.position);
        }

        VerificarVitoria();
    }

    void VerificarVitoria()
    {
        for (int l = 0; l < totalLinhas; l++)
        {
            for (int c = 0; c < totalColunas; c++)
            {
                if (matrizAtual[l, c] != matrizGabarito[l, c]) 
                {
                    return;
                }
            }
        }
        
        Debug.Log("VOCE GANHOU! Bandeira concluida perfeitamente!");
        if (fiosBloqueio != null) fiosBloqueio.SetActive(false);
        
        FecharPuzzle();
    }
}