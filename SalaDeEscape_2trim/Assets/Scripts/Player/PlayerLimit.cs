using UnityEngine;
using System.Collections;

public class PlayerLimit : MonoBehaviour
{
    public GameObject Player;
    public Transform res1;
    public Transform res2;
    public GameObject Limitt;
    [SerializeField]public bool currRespawn1;
    [SerializeField]public bool currRespawn2;
    private void Awake()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            print("respawn set");
            if (currRespawn2 == true)
            {
                Player.transform.position = res2.transform.position;
                currRespawn1 = false;
            }
            else if (currRespawn1 == true)
            {
                Player.transform.position = res1.transform.position;
                currRespawn2 = false;
            }
        }
    }
}