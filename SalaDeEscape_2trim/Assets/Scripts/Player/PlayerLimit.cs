using UnityEngine;
using System.Collections;

public class PlayerLimit : MonoBehaviour
{
    public GameObject Player;
    public Transform res1;
    public Transform res2;
    public GameObject Limitt;
    private int currRespawn;
    private void Awake()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            print("FFFFF");
            if (currRespawn == 1 || currRespawn == 0)
            {
                Player.transform.position = res1.transform.position;
            }
            else if (currRespawn == 2)
            {
                Player.transform.position = res2.transform.position;
            }
        }
    }

    public void QualSpawnEsta(int nRespawn)
    {
        currRespawn = nRespawn;
        print("" + currRespawn);
    }
}