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
    private int indiceAnterior = -1;
    private int repeticoesSeguidas = 0;
    public int maxRepeticoesToleradas = 2;

    [Header("Tempo de Espera nos Pontos")]
    public float tempoEsperaMin = 1.0f;
    public float tempoEsperaMax = 3.0f;
    private bool estaEsperandoNoPonto = false;

    [Header("Configurações de Visão (Susto)")]
    public float raioVisao = 10f;
    public LayerMask camadasBloqueadoras;
    public float tempoDelay = 0.5f;

    private bool estaSumindo = false; 
    private float timerDebug = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Busca o Player pela Tag se o campo estiver vazio
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Garante que o agente comece destravado
        if (agent != null)
        {
            agent.isStopped = false;
        }

        SortearProximoPonto();
    }

    

void Update()
{
    // 1. TESTE DE CONEXÃO DO PLAYER
    if (player == null)
    {
        Debug.LogError("ERRO GRAVE: O campo 'Player' está VAZIO no Inspector!");
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
            Debug.Log("Player encontrado automaticamente pela Tag 'Player'!");
        }
        return;
    }

    // 2. DIAGNÓSTICO A CADA 1 SEGUNDO
    timerDebug += Time.deltaTime;
    if (timerDebug >= 1.0f)
    {
        float dist = Vector3.Distance(transform.position, player.position);
        Debug.Log($"[DIAGNÓSTICO] Distância real: {dist:F1}m | Raio necessário: {raioVisao}m | Tá sumindo agora? {estaSumindo}");
        timerDebug = 0f;
    }

    if (estaSumindo) return;

    float distanciaParaOPlayer = Vector3.Distance(transform.position, player.position);

    // Se a distância for menor ou igual ao raio
    if (distanciaParaOPlayer <= raioVisao)
    {
        Debug.Log("Player entrou no raio! Testando visão...");

        if (TemLinhaDeVisaoDireta())
        {
            Debug.Log("VISÃO LIMPA! Iniciando teletransporte!");
            StopAllCoroutines();
            estaEsperandoNoPonto = false;
            StartCoroutine(RotinaTeletransporte());
        }
    }
    else
    {
        // Patrulha normal
        if (agent.enabled && agent.isOnNavMesh)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.3f)
            {
                if (!estaEsperandoNoPonto)
                {
                    StartCoroutine(EsperaNoPonto());
                }
            }
        }
    }
}

bool TemLinhaDeVisaoDireta()
{
    Vector3 origemRaio = transform.position + Vector3.up * 1.5f;
    Vector3 posicaoAlvo = player.position + Vector3.up * 1.0f;
    Vector3 direcao = (posicaoAlvo - origemRaio).normalized;
    float distancia = Vector3.Distance(origemRaio, posicaoAlvo);

    // Linha VERMELHA visível na aba Scene durante o Play
    Debug.DrawLine(origemRaio, posicaoAlvo, Color.red);

    RaycastHit hit;

    // Dispara o raio testando colisão
    if (Physics.Raycast(origemRaio, direcao, out hit, distancia, camadasBloqueadoras, QueryTriggerInteraction.Ignore))
    {
        Debug.Log("Visão do Fantasma bloqueada pelo objeto: " + hit.transform.name + " (Layer: " + LayerMask.LayerToName(hit.transform.gameObject.layer) + ")");
        return false;
    }

    return true;
}

    IEnumerator EsperaNoPonto()
    {
        estaEsperandoNoPonto = true;
        float tempoSorteado = Random.Range(tempoEsperaMin, tempoEsperaMax);
        yield return new WaitForSeconds(tempoSorteado);

        SortearProximoPonto();
        estaEsperandoNoPonto = false;
    }

    IEnumerator RotinaTeletransporte()
    {
        estaSumindo = true;
        
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(tempoDelay);

        if (pontosPatrulha != null && pontosPatrulha.Length > 0)
        {
            Transform pontoMaisDistante = pontosPatrulha[0];
            float maiorDistancia = 0f;

            foreach (Transform ponto in pontosPatrulha)
            {
                if (ponto == null) continue;
                float distancia = Vector3.Distance(ponto.position, player.position);
                if (distancia > maiorDistancia)
                {
                    maiorDistancia = distancia;
                    pontoMaisDistante = ponto;
                }
            }

            // Força o teletransporte desativando e ativando o Agent para NÃO depender do Warp
            agent.enabled = false;
            transform.position = pontoMaisDistante.position;
            agent.enabled = true;

            Debug.Log("Teletransportado com sucesso para: " + pontoMaisDistante.name);
        }

        repeticoesSeguidas = 0;
        indiceAnterior = -1;

        if (agent.enabled)
        {
            agent.isStopped = false;
        }

        SortearProximoPonto();

        yield return new WaitForSeconds(0.5f);
        estaSumindo = false; // Destrava a busca no Update
    }

    /* bool TemLinhaDeVisaoDireta()
    {
        Vector3 origemRaio = transform.position + Vector3.up * 1.5f;
        Vector3 posicaoAlvo = player.position + Vector3.up * 1.0f;
        Vector3 direcao = (posicaoAlvo - origemRaio).normalized;
        float distancia = Vector3.Distance(origemRaio, posicaoAlvo);

        Debug.DrawLine(origemRaio, posicaoAlvo, Color.cyan);

        // Se houver parede/obstáculo no caminho, bloqueia a visão
        if (Physics.Raycast(origemRaio, direcao, distancia, camadasBloqueadoras, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    } */

    void SortearProximoPonto()
    {
        if (pontosPatrulha == null || pontosPatrulha.Length == 0) return;

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

        if (agent.enabled && agent.isOnNavMesh && pontosPatrulha[indiceAtual] != null)
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