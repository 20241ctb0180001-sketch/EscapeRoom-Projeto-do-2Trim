using UnityEngine;
using System.Collections;

public class PlayerLimit : MonoBehaviour
{
    public GameObject Player;
    public Transform res1;
    public Transform res2;
    public GameObject Limitt;
    // private Transform currRespawn;
    private int currRespawn;
    private void Awake()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");

        /*if (currRespawn == null && res1 != null)
            currRespawn = res1;*/
    }

    void Update()
    {
        /*if(Player.transform.position.y <= Limitt.transform.position.y)
        {
            Player.transform.position = currRespawn.transform.position;
        }*/
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            print("FFFFF");
            if (currRespawn == 1)
            {
                Player.transform.position = res1.transform.position;
            }
            else if (currRespawn == 2)
            {
                Player.transform.position = res2.transform.position;
            }
            //Player.transform.position = currRespawn.transform.position;
        }
        /*else if (other.gameObject == res2)
        {
            currRespawn = res2;
        }*/

        /*if (other.CompareTag("Player") && currRespawn != null)
        {
            Player.transform.position = currRespawn.transform.position;
        }*/
    }

    public void QualSpawnEsta(int nRespawn)
    {
        currRespawn = nRespawn;
        print("" + currRespawn);
        /*if (nRespawn == 1)
        {
            currRespawn = res1;
        }
        else if (nRespawn == 2)
        {
            currRespawn = res2;
        }*/
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
