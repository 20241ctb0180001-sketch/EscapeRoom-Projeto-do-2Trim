using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class painelManager : MonoBehaviour
{
    [SerializeField] public static painelManager instance;

    [Header("Estado do puzzle")]
    [SerializeField] public bool puzzleAtivo = false;

    [Header("Configuracoes de cores")]
    [SerializeField] private int corSelecionada = -1; // -1 = Nenhuma cor selecionada
    
    [Tooltip("Arrastar os materiais na ordem exata")]
    [SerializeField] private Material[] materiaisCores; 

    [Header("Objeto do cenario")]
    [SerializeField] private GameObject fiosBloqueio; // Objeto dos fios da porta
    
    [Header("Camera")]
    [SerializeField] private FirstPersonLook look;
    private Camera mainCam;

    private int[,] matrizAtual = new int[8, 8];

    [Header("Carta de Dica")]
    [SerializeField] private Item cartaItem; //colocar o item carta aqui
    [SerializeField] private GameObject cartaNaParede; // objeto ja posicionado na parede, desativado por padrao
    [SerializeField] private PlayerInventory inventoryPlayer; // arrastar o Player no Inspector


    [Header("Audio")]
    [SerializeField] private EventReference somBotao;
    [SerializeField] private EventReference somBolinha;
    

    // Gabarito da Bandeira (8 faixas de cores)
    private int[,] matrizGabarito = new int[8, 8] {
        { 0, 0, 0, 0, 0, 0, 0, 0 }, // bola 1: Branco (0)
        { 0, 1, 1, 1, 1, 1, 1, 0 }, // bola 2: Vermelho (1)
        { 0, 2, 2, 2, 2, 2, 2, 0 }, // bola 3: Laranja (2)
        { 0, 4, 4, 4, 4, 4, 4, 0 }, // bola 4: Amarelo (4)
        { 0, 6, 6, 6, 6, 6, 6, 0 }, // bola 5: Verde (6)
        { 0, 3, 3, 3, 3, 3, 3, 0 }, // bola 6: Azul (3)
        { 0, 5, 5, 5, 5, 5, 5, 0 }, // bola 7: Roxo (5)
        { 0, 0, 0, 0, 0, 0, 0, 0 }  // bola 8: Branco (0)
    };

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCam = Camera.main;
        ResetarMatrizLimpa(); // Sincroniza a memoria com o estado zerado da tela
    }

    void ResetarMatrizLimpa()
    {
        for (int l = 0; l < 8; l++)
        {
            for (int c = 0; c < 8; c++)
            {
                matrizAtual[l, c] = -1; // Marca tudo como indifenido(-1)
            }
        }
    }
    

    public void AbrirPuzzle()
    {
        puzzleAtivo = true;
        if (look != null) look.enabled = false;
        /*Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;*/

        if (cartaNaParede != null && cartaItem != null && inventoryPlayer != null && inventoryPlayer.Itens.Contains(cartaItem))
        {
            cartaNaParede.SetActive(true);
        }

        Debug.Log("Painel ativado!");
    }

    public void FecharPuzzle()
    {
        puzzleAtivo = false;
        if (look != null) look.enabled = true;
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
        Debug.Log("Painel desativado!");
    }

    void Update()
    {
            if (!puzzleAtivo) return;

        bool apertouEsc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool apertouBotaoDireito = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;

        if (apertouEsc || apertouBotaoDireito)
        {
            FecharPuzzle();
            return;
        }

        bool apertouBotaoEsquerdo = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (apertouBotaoEsquerdo) 
        {
            ProcessarClique();
        }
    }

    void ProcessarClique()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
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
                    Debug.LogWarning("Selecione uma cor primeiro!");
                    return;
                }

                int linha = 0;
                int coluna = 0;

                string[] partes = objClicado.name.Split(' ');

                if (partes.Length >= 2)
                {
                    int.TryParse(partes[1], out linha);
                    linha = Mathf.Max(0, linha - 1); // Evita ficar negativo
                }

                if (partes.Length >= 3)
                {
                    int.TryParse(partes[2], out coluna);
                    coluna = Mathf.Max(0, coluna - 1); // Evita ficar negativo
                }
                else
                {
                    coluna = 0;
                }

                Debug.Log($" Clicou em '{objClicado.name}' -> Matriz[{linha},{coluna}]");
                PintarBolinha(objClicado, linha, coluna);
            }
        }
    }

    /* int DescobrirIdPeloNome(string nomeObjeto)
    {
        string nomeUpper = nomeObjeto.ToUpper();
        if (nomeUpper.Contains("APAGAR") || nomeUpper.Contains("BOTAU_0")) return 0; // Branco/Apagar
        if (nomeUpper.Contains("BCOLORA") || nomeUpper.Contains("BOTAU_1")) return 1; // Vermelho
        if (nomeUpper.Contains("BCOLORB") || nomeUpper.Contains("BOTAU_2")) return 2; // Laranja
        if (nomeUpper.Contains("BCOLORC") || nomeUpper.Contains("BOTAU_3")) return 3; // Azul
        if (nomeUpper.Contains("BCOLORD") || nomeUpper.Contains("BOTAU_4")) return 4; // Amarelo
        if (nomeUpper.Contains("BCOLORE") || nomeUpper.Contains("BOTAU_5")) return 5; // Roxo
        if (nomeUpper.Contains("BCOLORF") || nomeUpper.Contains("BOTAU_6")) return 6; // Verde
        return -1;
    } */

    public void SelecionarCor(int idCor)
    {
        corSelecionada = idCor;
        Debug.Log("Cor ativa alterada com SUCESSO para ID: " + idCor);
    }

    void PintarBolinha(GameObject objetoBolinha, int linha, int coluna)
    {
        linha = Mathf.Clamp(linha, 0, 7);
        coluna = Mathf.Clamp(coluna, 0, 7);

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
        for (int l = 0; l < 8; l++)
        {
            for (int c = 0; c < 8; c++)
            {
                // Se QUALQUER celula estiver diferente do gabarito, cancela a vitoria
                if (matrizAtual[l, c] != matrizGabarito[l, c]) 
                {
                    Debug.Log($" Nao deu vitoria ainda! Celula [{l},{c}] esta com a cor ID {matrizAtual[l, c]}, mas o gabarito espera a cor ID {matrizGabarito[l, c]}.");
                    return;
                }
            }
        }
        
        // Se passou por TODAS as 64 posicoes sem nenhuma errada:
        Debug.Log("VOCE GANHOU! Bandeira concluida perfeitamente!");
        if (fiosBloqueio != null) fiosBloqueio.SetActive(false);
        
        FecharPuzzle();
    }
}