using UnityEngine;
using UnityEngine.Events;

public class RandoStairs : MonoBehaviour
{
    [SerializeField] private GameObject esseDegrau;
    public GameObject hand;
    public UnityEvent EE1;
    public UnityEvent EE2;

    void Awake()
    {
        esseDegrau = gameObject;
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
                    Debug.Log("easterEgg1");
                    EE1.Invoke();
                    break;

                case 4:
                    EE2.Invoke();
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
    }
}
