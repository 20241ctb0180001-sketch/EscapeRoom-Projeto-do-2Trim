using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class painelManager : MonoBehaviour
{
    [SerializeField] public static painelManager instance;

    [Header("Estado do puzzle")]
    [SerializeField] public bool puzzleAtivo = false; // verifica se o puzzle esta ativo

    [Header("Configuracoes de cores")]
    [SerializeField] private int corSelecionada = -1; // Guarda o ID da cor que o jogador clicou por ultimo (-1 significa que nenhuma foi escolhida ainda)
    
    [Tooltip("Arrastar os materiais na ordem exata")]
    [SerializeField] private Material[] materiaisCores;  // cria uma listinha de elementos no inspector pra arrastar os materiais

    [Header("Objeto do cenario")]
    [SerializeField] private GameObject fiosBloqueio; // Objeto dos fios da porta
    
    [Header("Camera")]
    [SerializeField] private FirstPersonLook look; // pega a visão do player
    private Camera mainCam;

    private int[,] matrizAtual; // fica atualizando pra ver como ta a matriz atual

    [Header("Carta de Dica")]
    [SerializeField] private Item cartaItem; // colocar o item carta aqui
    [SerializeField] private GameObject cartaNaParede; // objeto ja posicionado na parede, desativado por padrao
    [SerializeField] private PlayerInventory inventoryPlayer; // arrastar o Player no Inspector

    [Header("Audio")]
    [SerializeField] private EventReference somBotao; // coisinha pro som do botão
    [SerializeField] private EventReference somBolinha; // coisinha pro som das bolinhas

    // Ações do Input System carregadas dinamicamente pelos nomes do seu Asset
    private InputAction acaoInteractMouse;
    private InputAction acaoSairPuzzle;

    // Gabarito da Bandeira (8 faixas de cores) - Define dinamicamente a dimensão da matriz
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

        // Inicializa as dimensões de forma dinâmica a partir da matriz do gabarito sem ter valores hardcoded
        totalLinhas = matrizGabarito.GetLength(0);
        totalColunas = matrizGabarito.GetLength(1);
        matrizAtual = new int[totalLinhas, totalColunas];

        // Busca automaticamente as ações do seu Input System pelos nomes exatos da sua lista
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            acaoInteractMouse = playerInput.actions.FindAction("InteractMouse");
            acaoSairPuzzle = playerInput.actions.FindAction("SairPuzzle");
        }
    }

    void Start()
    {
        mainCam = Camera.main;
        ResetarMatrizLimpa(); // Inicia a matriz zerada
    }

    void ResetarMatrizLimpa() // Preenche a matriz do jogador com -1 (sem cor atribuida)
    {
        for (int l = 0; l < totalLinhas; l++)
        {
            for (int c = 0; c < totalColunas; c++)
            {
                matrizAtual[l, c] = -1; // Marca tudo como indifenido(-1)
            }
        }
    }

    public void AbrirPuzzle()
    {
        puzzleAtivo = true;
        if (look != null) look.enabled = false;
        
        // Força o sumiço da patinha na UI do seu GerentUI
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

        // Tenta usar a ação 'SairPuzzle' configurada no asset; faz fallback direto no teclado/mouse se não achar
        bool apertouSair = (acaoSairPuzzle != null && acaoSairPuzzle.WasPressedThisFrame()) ||
                           (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
                           (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame);

        if (apertouSair)
        {
            FecharPuzzle();
            return;
        }

        // Tenta usar a ação 'InteractMouse' configurada no asset; faz fallback no clique esquerdo se não achar
        bool apertouClique = (acaoInteractMouse != null && acaoInteractMouse.WasPressedThisFrame()) ||
                            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (apertouClique)
        {
            ProcessarClique();
        }
    }

    // Dispara um Raycast da tela ate o cenario para saber em qual objeto o player clicou
    void ProcessarClique()
    {
        Vector2 mousePos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f)) // ve com o raycast o objeto em que foi clicado
        {
            GameObject objClicado = hit.collider.gameObject;

            // 1. BOTAO DE COR
            BotaoCor botao = objClicado.GetComponent<BotaoCor>(); // pega o componente id do botão cor(se for ele) e chama o selecionar cor
            if (botao != null)
            {
                SelecionarCor(botao.CorId);
                if (!somBotao.IsNull) RuntimeManager.PlayOneShot(somBotao, hit.point); // toca somzinho
                return;
            }

            // 2. BOLINHA
            if (objClicado.name.ToLower().StartsWith("bola") || objClicado.CompareTag("BolinhaPainel") || objClicado.name.StartsWith("Sphere")) // verifica se o objeto começa com "bola, se tem tag bolinha painel ou se começa com esfera)
            {
                if (corSelecionada == -1) // se nao foi selecionado uma cor antes, ela continua como -1 e nao faz nada)
                {
                    Debug.LogWarning("Selecione uma cor primeiro!");
                    return;
                }

                int linha = 0;
                int coluna = 0;

                string[] partes = objClicado.name.Split(' '); // Divide o nome do objeto por espacos para extrair a coordenada ("Bola 2 4")

                if (partes.Length >= 2) // basicamente ta dividindo o nome em partes e transformando em numeros inteiros, pra ajustar na matriz certo e dai chamar o pintarbolinhas
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
                PintarBolinha(objClicado, linha, coluna); // chama o pintar bolinha pra a bolinha que foi clicada
            }
        }
    }

    public void SelecionarCor(int idCor) // ve o idCor do botão selecionado e guarda em corSelecionada
    {
        corSelecionada = idCor;
        Debug.Log("Cor ativa alterada com SUCESSO para ID: " + idCor);
    }

    void PintarBolinha(GameObject objetoBolinha, int linha, int coluna) // Atualiza a memoria logica e altera visualmente a cor da bolinha
    {
        linha = Mathf.Clamp(linha, 0, totalLinhas - 1);
        coluna = Mathf.Clamp(coluna, 0, totalColunas - 1);

        matrizAtual[linha, coluna] = corSelecionada; // Salva a cor escolhida na posição exata da matriz lógica do jogador

        MeshRenderer renderer = objetoBolinha.GetComponent<MeshRenderer>(); // Pega o componente visual da esfera (MeshRenderer)
        if (renderer != null && corSelecionada >= 0 && corSelecionada < materiaisCores.Length) // Valida se o renderer existe e se o ID da cor é valido dentro da lista de materiais
        {
            renderer.material = materiaisCores[corSelecionada]; // Troca o material 3D da esfera pela cor selecionada
            if (!somBolinha.IsNull) RuntimeManager.PlayOneShot(somBolinha, objetoBolinha.transform.position); // toca somzinho
        }

        VerificarVitoria();
    }

    void VerificarVitoria() // Percorre as matrizes e compara a matriz atual com o matriz gabarito
    {
        for (int l = 0; l < totalLinhas; l++)
        {
            for (int c = 0; c < totalColunas; c++)
            {
                // Se QUALQUER celula estiver diferente do gabarito, cancela a vitoria
                if (matrizAtual[l, c] != matrizGabarito[l, c]) 
                {
                    Debug.Log($" Nao deu vitoria ainda! Celula [{l},{c}] esta com a cor ID {matrizAtual[l, c]}, mas o gabarito espera a cor ID {matrizGabarito[l, c]}.");
                    return;
                }
            }
        }
        
        // Se passou por TODAS as posicoes da matriz sem nenhuma errada:
        Debug.Log("VOCE GANHOU! Bandeira concluida perfeitamente!");
        if (fiosBloqueio != null) fiosBloqueio.SetActive(false); // verifica se os fios nao estao vazios e se nao tiver desativa eles
        
        FecharPuzzle();
    }
}