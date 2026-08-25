using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IAFantasmaComEspera : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Alvo")]
    public Transform player;

    [Header("Configurações de Patrulha")]
    public Transform[] pontosPatrulha;
    private int indiceAtual = -1;

    [Header("Tempo de Espera nos Pontos")]
    public float tempoEsperaMin = 2.0f; // Tempo mínimo de parada
    public float tempoEsperaMax = 5.0f; // Tempo máximo de parada
    private bool estaEsperandoNoPonto = false; // Evita iniciar várias esperas ao mesmo tempo

    [Header("Configurações de Visão (Susto)")]
    public float raioVisao = 10f;
    public LayerMask camadasBloqueadoras;
    public float tempoDelay = 1.0f;

    private bool estaSumindo = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

            StartCoroutine(RotinaTeletransporte());
        }
        else
        {
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

    IEnumerator RotinaTeletransporte()
    {
        estaSumindo = true; 
        agent.isStopped = true; 

        yield return new WaitForSeconds(tempoDelay);

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

            agent.Warp(pontoMaisDistante.position);
        }

        agent.isStopped = false; 
        SortearProximoPonto();

        estaSumindo = false; 
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
            if (agent.enabled && agent.isOnNavMesh)
                agent.SetDestination(pontosPatrulha[0].position);
            return;
        }

        int novoIndice = indiceAtual;
        while (novoIndice == indiceAtual)
        {
            novoIndice = Random.Range(0, pontosPatrulha.Length);
        }

        indiceAtual = novoIndice;
        
        if (agent.enabled && agent.isOnNavMesh)
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