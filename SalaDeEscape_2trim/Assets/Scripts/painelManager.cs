using UnityEngine;

public class painelManager : MonoBehaviour
{
    public static painelManager instance;

    [Header("Estado do puzzle")]
    public bool puzzleAtivo = false;

    [Header("Configuracoes de cores")]
    public int corSelecionada = -1; // -1 = Nenhuma cor selecionada
    
    [Tooltip("Arrastar os materiais na ordem exata")]
    public Material[] materiaisCores; 

    [Header("Objeto do cenario")]
    public GameObject fiosBloqueio; // Objeto dos fios da porta
    
    [Header("Camera")]
    public FirstPersonLook look;

    private Camera mainCam;
    private int[,] matrizAtual = new int[8, 8];

    [Header("Carta de Dica")]
    public Item cartaItem;
    public GameObject cartaNaParede; // objeto ja posicionado na parede, desativado por padrao
    public PlayerInventory inventoryPlayer; // arraste o Player no Inspector

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
                matrizAtual[l, c] = 0; // Marca tudo como Branco (ID 0)
            }
        }
    }
    

    public void AbrirPuzzle()
    {
        puzzleAtivo = true;
        if (look != null) look.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cartaNaParede != null && cartaItem != null && inventoryPlayer != null && inventoryPlayer.itens.Contains(cartaItem))
        {
            cartaNaParede.SetActive(true);
        }

        Debug.Log("Painel ativado!");
    }

    public void FecharPuzzle()
    {
        puzzleAtivo = false;
        if (look != null) look.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Painel desativado!");
    }

    void Update()
    {
        if (!puzzleAtivo) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            FecharPuzzle();
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            ProcessarClique();
        }
    }

    void ProcessarClique()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject objClicado = hit.collider.gameObject;

            // 1. BOTAO DE COR
            if (objClicado.GetComponent<UnityEngine.EventSystems.EventTrigger>() != null || objClicado.CompareTag("BotaoCor") || objClicado.name.StartsWith("BColor"))
            {
                int idIdentificado = DescobrirIdPeloNome(objClicado.name);
                if (idIdentificado != -1)
                {
                    SelecionarCor(idIdentificado);
                    return;
                }
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

    int DescobrirIdPeloNome(string nomeObjeto)
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
    }

    public void SelecionarCor(int idCor)
    {
        corSelecionada = idCor;
        Debug.Log("Cor ativa alterada com SUCESSO para ID: " + idCor);
    }

    void PintarBolinha(GameObject objetoBolinha, int linha, int coluna)
    {
        // Garante estritamente que linha e coluna fiquem entre 0 e 7 (Evita IndexOutOfRange!)
        linha = Mathf.Clamp(linha, 0, 7);
        coluna = Mathf.Clamp(coluna, 0, 7);

        matrizAtual[linha, coluna] = corSelecionada;

        MeshRenderer renderer = objetoBolinha.GetComponent<MeshRenderer>();
        if (renderer != null && corSelecionada >= 0 && corSelecionada < materiaisCores.Length)
        {
            renderer.material = materiaisCores[corSelecionada];
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