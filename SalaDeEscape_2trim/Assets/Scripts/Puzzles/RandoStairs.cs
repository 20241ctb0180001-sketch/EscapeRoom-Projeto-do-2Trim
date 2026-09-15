using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;

public class RandoStairs : MonoBehaviour
{
    [SerializeField] private GameObject esseDegrau;
    public GameObject hand;
    public UnityEvent EE2;
    public GameObject Deco;
    public string qMadeira = "event:/Quebra madeira";

    [Header("Easter Egg - UI")]
    [SerializeField] private GameObject uiGatoGirando; // Drag and drop do objeto da UI aqui!
    [SerializeField] private float tempoExibicaoGato = 3f; // Tempo em segundos que ficará na tela

    void Awake()
    {
        esseDegrau = gameObject;
        Deco = GameObject.FindGameObjectWithTag("deco");
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
    {
        // Sorteia um número de 1 a 100 (100 opções de porcentagem)
        int rng = Random.Range(1, 101);

        // CASO 1: Quebrar a escada (50% de chance -> números de 1 a 50)
        if (rng <= 50)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            PlayBreakAudio();
            Debug.Log("Desaparece");
        }
        // CASO 2: Mão (Apenas 5% de chance -> números de 51 a 55)
        else if (rng > 50 && rng <= 55)
        {
            SpawnObject();
            Debug.Log("mao");
        }
        // CASO 3: Decoração (15% de chance -> números de 56 a 70)
        else if (rng > 55 && rng <= 70)
        {
            bool state = !Deco.activeInHierarchy;
            SetActiveDeco(state);
        }
        // CASO 4: Gato Girando (10% de chance -> números de 71 a 80)
        else if (rng > 70 && rng <= 80)
        {
            StartCoroutine(MostrarEasterEggUI());
            StartCoroutine(InvokeEE2(EE2, 3f));
            Debug.Log("easterEgg2 - Gato");
        }
        // CASO DEFAULT: Nada acontece (20% de chance -> números de 81 a 100)
    }
    }

    void SpawnObject()
    {
        Vector3 spawnPosition = esseDegrau.transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        Instantiate(hand, spawnPosition, spawnRotation);
        Destroy(hand, 1.5f);
    }

    private IEnumerator InvokeEE2(UnityEvent unityEvent, float duration)
    {
        unityEvent.Invoke();
        yield return new WaitForSeconds(duration);
    }

    // COROUTINE DO EASTER EGG NA UI
    private IEnumerator MostrarEasterEggUI()
    {
        if (uiGatoGirando != null)
        {
            uiGatoGirando.SetActive(true); // Ativa a UI (isso inicia a animação e o AudioSource se Play On Awake estiver ligado)
            
            // Garante que o som toque caso Play On Awake não esteja ativo no AudioSource
            AudioSource audio = uiGatoGirando.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }

            yield return new WaitForSeconds(tempoExibicaoGato); // Espera o tempo configurado (ex: 3s)
            
            uiGatoGirando.SetActive(false); // Esconde a UI e para o som/animação
        }
    }

    void SetActiveDeco(bool state)
    {
        Deco.SetActive(state);
    }

    void PlayBreakAudio() => RuntimeManager.PlayOneShot(qMadeira, transform.position);
}
