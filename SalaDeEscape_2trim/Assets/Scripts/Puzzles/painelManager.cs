using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class painelManager : MonoBehaviour
{
    [SerializeField] public static painelManager instance;

    [Header("Estado do puzzle")]
    [SerializeField] public bool puzzleAtivo = false; //verifica se o puzzle esta ativo

    [Header("Configuracoes de cores")]
    [SerializeField] private int corSelecionada = -1; // Guarda o ID da cor que o jogador clicou por ultimo (-1 significa que nenhuma foi escolhida ainda)
    
    [Tooltip("Arrastar os materiais na ordem exata")]
    [SerializeField] private Material[] materiaisCores;  //cria uma listinha de elementos no inspector pra arrastar os materiais

    [Header("Objeto do cenario")]
    [SerializeField] private GameObject fiosBloqueio; // Objeto dos fios da porta
    
    [Header("Camera")]
    [SerializeField] private FirstPersonLook look; //pega a visão do palyer
    private Camera mainCam;

    private int[,] matrizAtual = new int[8, 8]; //fica atualizando pra ver como ta a matriz atual

    [Header("Carta de Dica")]
    [SerializeField] private Item cartaItem; //colocar o item carta aqui
    [SerializeField] private GameObject cartaNaParede; // objeto ja posicionado na parede, desativado por padrao
    [SerializeField] private PlayerInventory inventoryPlayer; // arrastar o Player no Inspector


    [Header("Audio")]
    [SerializeField] private EventReference somBotao; //coisinha pro som do botão
    [SerializeField] private EventReference somBolinha; //coisinha pro som das bolinhas
    

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
        ResetarMatrizLimpa(); // Inicia a matriz zerada
    }

    void ResetarMatrizLimpa() // Preenche a matriz do jogador com -1 (sem cor atribuida)
    {
        for (int l = 0; l < 8; l++)
        {
            for (int c = 0; c < 8; c++)
            {
                matrizAtual[l, c] = -1; // Marca tudo como indifenido(-1)
            }
        }
    }
    

    public void AbrirPuzzle() // Ativa a interface do puzzle, libera o mouse e pausa a camera do jogador
    {
        puzzleAtivo = true;
        if (look != null) look.enabled = false;
        /*Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;*/

        if (cartaNaParede != null && cartaItem != null && inventoryPlayer != null && inventoryPlayer.Itens.Contains(cartaItem)) // Revela a dica na parede caso o jogador possua o Item no inventario
        {
            cartaNaParede.SetActive(true);
        }

        Debug.Log("Painel ativado!");
    }

    public void FecharPuzzle() // Fecha o puzzle e devolve o controle do mouse para a camera de 1ª pessoa
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

        bool apertouEsc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame; //verifica se apertou esc
        bool apertouBotaoDireito = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame; //verifica se apertou o botão direito de acordo com o Input System

        if (apertouEsc || apertouBotaoDireito) //fecha o puzzle se alguem dos dois coisas foram apertados
        {
            FecharPuzzle();
            return;
        }

        bool apertouBotaoEsquerdo = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (apertouBotaoEsquerdo)  //se apertou o botão esquerdo, chama o processar clique
        {
            ProcessarClique();
        }
    }

    // Dispara um Raycast da tela ate o cenario para saber em qual objeto o player clicou
    void ProcessarClique()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f)) //ve com o raycast o objeto em que foi clicado
        {
            GameObject objClicado = hit.collider.gameObject;

            // 1. BOTAO DE COR
            BotaoCor botao = objClicado.GetComponent<BotaoCor>(); //pega o componente id do botão cor(se for ele) e chama o selecionar cor
            if (botao != null)
            {
                SelecionarCor(botao.CorId);
                if (!somBotao.IsNull) RuntimeManager.PlayOneShot(somBotao, hit.point); //toca somzinho
                return;
            }

            // 2. BOLINHA
            if (objClicado.name.ToLower().StartsWith("bola") || objClicado.CompareTag("BolinhaPainel") || objClicado.name.StartsWith("Sphere")) //verifica se o objeto começa com "bola, se tem tag bolinha painel ou se começa com esfera)
            {
                if (corSelecionada == -1)//se nao foi selecionado uma cor antes, ela continua como -1 e nao faz nada)
                {
                    Debug.LogWarning("Selecione uma cor primeiro!");
                    return;
                }

                int linha = 0;
                int coluna = 0;

                string[] partes = objClicado.name.Split(' '); // Divide o nome do objeto por espacos para extrair a coordenada ("Bola 2 4")

                if (partes.Length >= 2) //basicamente ta dividindo o nome em brates e transformando em numeros inteiros, pra ajustar na matriz certo e dai chamar o pintarbolinhas
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
                PintarBolinha(objClicado, linha, coluna); //chama o pintar bolinha pra a bolinha que foi clicada
            }
        }
    }

    public void SelecionarCor(int idCor) //ve o idCor do botão selecionado e guarda em corSelecionada
    {
        corSelecionada = idCor;
        Debug.Log("Cor ativa alterada com SUCESSO para ID: " + idCor);
    }

    void PintarBolinha(GameObject objetoBolinha, int linha, int coluna) // Atualiza a memoria logica e altera visualmente a cor da bolinha
    {
        linha = Mathf.Clamp(linha, 0, 7);
        coluna = Mathf.Clamp(coluna, 0, 7);

        matrizAtual[linha, coluna] = corSelecionada; // Salva a cor escolhida na posição exata da matriz lógica do jogador

        MeshRenderer renderer = objetoBolinha.GetComponent<MeshRenderer>(); // Pega o componente visual da esfera (MeshRenderer)
        if (renderer != null && corSelecionada >= 0 && corSelecionada < materiaisCores.Length) // Valida se o renderer existe e se o ID da cor é valido dentro da lista de materiais
        {
            renderer.material = materiaisCores[corSelecionada]; // Troca o material 3D da esfera pela cor selecionada
            if (!somBolinha.IsNull) RuntimeManager.PlayOneShot(somBolinha, objetoBolinha.transform.position); //toca somzinho
        }

        VerificarVitoria();
    }

    void VerificarVitoria() //Percorre as matrizes e compara a matriz atual com o matriz gabarito
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
        if (fiosBloqueio != null) fiosBloqueio.SetActive(false); //verifica se os fios nao estao vazios e se nao tiver desativa eles
        
        FecharPuzzle();
    }
}