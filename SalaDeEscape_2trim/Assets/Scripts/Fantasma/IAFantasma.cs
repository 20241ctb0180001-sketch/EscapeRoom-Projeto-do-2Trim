using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IAFantasma : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Alvo")]
    public Transform player;

    [Header("Configurações de Patrulha")]
    public Transform[] pontosPatrulha;
    private int indiceAtual = -1;
    private int indiceAnterior = -1; // Guarda de onde ele acabou de vir
    private int repeticoesSeguidas = 0; // Quantas vezes alternou entre os mesmos 2 pontos
    public int maxRepeticoesToleradas = 2; // Limite de vezes que pode ir e voltar entre os mesmos 2 pontos

    [Header("Tempo de Espera nos Pontos")]
    public float tempoEsperaMin = 2.0f; // Tempo mínimo de parada
    public float tempoEsperaMax = 5.0f; // Tempo máximo de parada
    private bool estaEsperandoNoPonto = false; // Evita iniciar várias esperas ao mesmo tempo

    [Header("Configurações de Visão (Susto)")]
    public float raioVisao = 10f;
    public LayerMask camadasBloqueadoras;
    public float tempoDelay = 1.0f;

    // Variável para evitar que o teletransporte seja ativado várias vezes seguidas durante o delay
    private bool estaSumindo = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        /*GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }*/

        SortearProximoPonto();
    }

    void Update()
    {
        if (player == null || estaSumindo) return;

        float distanciaParaOPlayer = Vector3.Distance(transform.position, player.position);

        if (distanciaParaOPlayer <= raioVisao && TemLinhaDeVisaoDireta())
        {
            // Se o fantasma se assustar enquanto esperava no ponto, cancelamos a espera
            StopAllCoroutines();
            estaEsperandoNoPonto = false;

            // Inicia o processo de sumir com delay
            StartCoroutine(RotinaTeletransporte());
        }
        else
        {

            /*if (agent.enabled && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SortearProximoPonto();
            }*/

            // Checa se chegou ao ponto e ainda não começou a esperar
            if (agent.enabled && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                if (!estaEsperandoNoPonto)
                {
                    StartCoroutine(EsperaNoPonto());
                }
            }
        }

    }

    IEnumerator EsperaNoPonto()
    {
        estaEsperandoNoPonto = true;

        // Sorteia um tempo aleatório entre o mínimo e o máximo configurados
        float tempoSorteado = Random.Range(tempoEsperaMin, tempoEsperaMax);
        
        yield return new WaitForSeconds(tempoSorteado);

        SortearProximoPonto();
        estaEsperandoNoPonto = false;
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

            //* Teletransporte físico
            agent.enabled = false;
            transform.position = pontoMaisDistante.position;
            agent.enabled = true;// */

            // agent.Warp(pontoMaisDistante.position);
        }
        
        // Reseta o contador ao se teletransportar para não travar sorteios novos
        repeticoesSeguidas = 0;
        indiceAnterior = -1;

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
        
        if (pontosPatrulha.Length >= 3 && repeticoesSeguidas >= maxRepeticoesToleradas)
        {

            while (novoIndice == indiceAtual || novoIndice == indiceAnterior)
            {
                novoIndice = Random.Range(0, pontosPatrulha.Length);
            }

            repeticoesSeguidas = 0; // Reseta o contador de repetições

        }else{

            // Sorteia normalmente (só garante que não seja igual ao ponto onde ele já está)
            while (novoIndice == indiceAtual)
            {
                novoIndice = Random.Range(0, pontosPatrulha.Length);
            }

            // Checa se o ponto sorteado foi justamente o ponto de onde ele veio na rodada passada
            if (novoIndice == indiceAnterior)
            {
                repeticoesSeguidas++;
            }
            else
            {
                // Se foi para um ponto inédito, reseta o contador de ping-pong
                repeticoesSeguidas = 0;
            }
        }

        // Atualiza os ponteiros de histórico
        indiceAnterior = indiceAtual;
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