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
            int rng = Random.Range(1, 11); // 10% de chance para cada evento
            switch (rng)
            {
                case 1:
                    gameObject.GetComponent<Collider>().enabled = false;
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    PlayBreakAudio();
                    Debug.Log("Disaparece");
                    break;

                case 2:
                    SpawnObject();
                    Debug.Log("mao");
                    break;

                case 3:
                    bool state = !Deco.activeInHierarchy;
                    SetActiveDeco(state);
                    break;

                case 4:
                    // Chama a Coroutine para mostrar o elemento na tela e sumir depois
                    StartCoroutine(MostrarEasterEggUI());
                    StartCoroutine(InvokeEE2(EE2, 3f));
                    Debug.Log("easterEgg2");
                    break;

                default:
                    break;
            }
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
