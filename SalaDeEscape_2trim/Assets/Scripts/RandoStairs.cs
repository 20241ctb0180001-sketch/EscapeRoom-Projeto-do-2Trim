using UnityEngine;
using UnityEngine.Events;

public class RandoStairs : MonoBehaviour
{
    [SerializeField] private GameObject esseDegrau;
    public GameObject hand;
    public UnityEvent handtap;
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
            int rng = Random.Range(1, 4);
            switch (rng)
            {
                case 1:
                    gameObject.GetComponent<Collider>().enabled = false;
                    Debug.Log("Disaparece");
                    break;

                case 2:
                    handtap.Invoke();
                    SpawnObject();
                    Debug.Log("mão");
                    break;

                case 3:
                    EE1.Invoke();
                    Debug.Log("easterEgg1");
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
        // Spawns the prefab at coordinates (0, 2, 0) with no rotation
        Vector3 spawnPosition = new Vector3(0f, 2f, 0f);
        Quaternion spawnRotation = Quaternion.identity;

        Instantiate(hand, spawnPosition, spawnRotation);
    }
}
