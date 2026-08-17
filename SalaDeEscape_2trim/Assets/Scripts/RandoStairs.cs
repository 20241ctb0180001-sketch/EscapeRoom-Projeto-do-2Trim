using UnityEngine;
using UnityEngine.Events;

public class RandoStairs : MonoBehaviour
{
    [SerializeField]private GameObject esseDegrau;
    public GameObject hand;
    public UnityEvent handtap;
    public UnityEvent EE;

    void Awake()
    {
        esseDegrau = gameObject;
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            int rng = Random.Range(1,3);
            switch(rng)
            {
                case 1:
                gameObject.GetComponent<Collider>().enabled = false;
                Debug.Log("Disaparece");
                break;

                case 2:
                handtap.Invoke();
                Instantiate(hand);
                Debug.Log("mão");
                break;

                case 3: 
                EE.Invoke();
                Debug.Log("easterEgg");
                break;

                default:
                break;
            }
        }
    }
}
