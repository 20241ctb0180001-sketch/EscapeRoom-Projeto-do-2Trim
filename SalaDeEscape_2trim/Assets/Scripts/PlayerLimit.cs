using UnityEngine;
using System.Collections;
public class PlayerLimit : MonoBehaviour
{
    public GameObject Player;
    public GameObject res1;
    public GameObject res2;
    public GameObject currRespawn;

    private void Awake()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player1");

        if (currRespawn == null && res1 != null)
            currRespawn = res1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == res1)
        {
            currRespawn = res1;
        }
        else if (other.gameObject == res2)
        {
            currRespawn = res2;
        }

        if (other.CompareTag("Player1") && currRespawn != null)
        {
            Player.transform.position = currRespawn.transform.position;
        }
    }
}

/*using UnityEngine;
using System.Collections;
public class PlayerLimit : MonoBehaviour
{
    public GameObject Player;
    public GameObject res1;
    public GameObject res2;
    public GameObject currRespawn;
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag ("Player1");
    }

    void Update()
    {
        if(resp1 == true)
        {
            currRespawn = res1;
        } else if(resp2 == true)
        {
            currRespawn = res2;
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.transform.position = currRespawn.transform.position;
        }
    }
}*/
