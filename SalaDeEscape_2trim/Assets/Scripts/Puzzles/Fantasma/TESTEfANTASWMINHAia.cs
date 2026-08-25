using System.Collections; // Necessário para usar Coroutines (IEnumerator)
using UnityEngine;
using UnityEngine.AI;
public class TESTEfANTASWMINHAia : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform player;

    [Header("Configurações de Patrulha")]
    public Transform[] pontosPatrulha;
    private int indiceAtual = -1;

    [Header("Configurações de Visão (Susto)")]
    public float raioVisao = 10f;
    public LayerMask camadasBloqueadoras;
    public float tempoDelay = 1.0f;

    // Variável para evitar que o teletransporte seja ativado várias vezes seguidas durante o delay
    private bool estaSumindo = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        SortearProximoPonto();
    }

    void Update()
    {
        if (player == null || estaSumindo) return;

        float distanciaParaOPlayer = Vector3.Distance(transform.position, player.position);

        if (distanciaParaOPlayer <= raioVisao && TemLinhaDeVisaoDireta())
        {
            // Inicia o processo de sumir com delay
            StartCoroutine(RotinaTeletransporte());
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SortearProximoPonto();
            }
        }
    }

    // Esta é a Corrotina que gerencia o tempo
    IEnumerator RotinaTeletransporte()
    {
        estaSumindo = true; // Bloqueia o Update de rodar essa função de novo

        // Paramos o inimigo no lugar para ele "reagir" ao susto
        agent.isStopped = true; 

        // [OPCIONAL] Coloque aqui o comando para tocar uma animação de susto, se tiver
        // Ex: GetComponent<Animator>().SetTrigger("Susto");

        // FAZ O JOGO ESPERAR POR 1 SEGUNDO
        yield return new WaitForSeconds(tempoDelay);

        // --- O SEGUNDO PASSOU, HORA DE SUMIR ---

        if (pontosPatrulha.Length > 0)
        {
            Transform pontoMaisDistante = pontosPatrulha[0];
            float maiorDistancia = 0f;

            foreach (Transform ponto in pontosPatrulha)
            {
                float distanciaPontoAoPlayer = Vector3.Distance(ponto.position, player.position);
                if (distanciaPontoAoPlayer > maiorDistancia)
                {
                    maiorDistancia = distanciaPontoAoPlayer;
                    pontoMaisDistante = ponto;
                }
            }

            // Teletransporte físico
            agent.enabled = false;
            transform.position = pontoMaisDistante.position;
            agent.enabled = true;
        }

        // Destrava o agente para ele voltar a andar no novo ponto
        agent.isStopped = false; 
        SortearProximoPonto();

        estaSumindo = false; // Permite que ele se assuste novamente no futuro
    }

    bool TemLinhaDeVisaoDireta()
    {
        Vector3 direcaoParaOPlayer = (player.position - transform.position).normalized;
        float distanciaParaOPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 origemRaio = transform.position + Vector3.up * 1f; 

        RaycastHit hit;
        if (Physics.Raycast(origemRaio, direcaoParaOPlayer, out hit, distanciaParaOPlayer, camadasBloqueadoras))
        {
            return false; 
        }
        return true; 
    }

    void SortearProximoPonto()
    {
        if (pontosPatrulha.Length == 0) return;
        if (pontosPatrulha.Length == 1)
        {
            agent.SetDestination(pontosPatrulha[0].position);
            return;
        }

        int novoIndice = indiceAtual;
        while (novoIndice == indiceAtual)
        {
            novoIndice = Random.Range(0, pontosPatrulha.Length);
        }

        indiceAtual = novoIndice;
        
        if (agent.enabled)
        {
            agent.SetDestination(pontosPatrulha[indiceAtual].position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioVisao);
    }
}