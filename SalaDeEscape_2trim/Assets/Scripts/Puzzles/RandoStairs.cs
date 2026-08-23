using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RandoStairs : MonoBehaviour
{
    [SerializeField] private GameObject esseDegrau;
    public GameObject hand;
    //public UnityEvent EE1;
    public UnityEvent EE2;
    public GameObject Deco;

    void Awake()
    {
        esseDegrau = gameObject;
        Deco = GameObject.FindGameObjectWithTag("deco");
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int rng = Random.Range(1, 5);
            switch (rng)
            {
                case 1:
                    gameObject.GetComponent<Collider>().enabled = false;
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    Debug.Log("Disaparece");
                    break;

                case 2:
                    SpawnObject();
                    Debug.Log("mao");
                    break;

                case 3:
                    bool state = !Deco.activeInHierarchy;
                    SetActiveDeco(state);
                    /*Debug.Log("easterEgg1");
                    EE1.Invoke();*/
                    break;

                case 4:
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

    void SetActiveDeco(bool state)
    {
        Deco.SetActive(state);
    }
}
