using UnityEngine;

public class DefinirSpawn : MonoBehaviour
{
    [SerializeField]private int nuRespawn;
    public GameObject Player;
    public GameObject objetoAlvo;
    private PlayerLimit alvoScript;
    void Start()
    {
        if (objetoAlvo != null)
        {
            alvoScript = objetoAlvo.GetComponent<PlayerLimit>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "respawn1")
        {
            alvoScript.currRespawn1 = true;
            Debug.Log("Respawn1 ativado");
        }
        else if (gameObject.tag == "respawn2")
        {
            alvoScript.currRespawn2 = true;
            Debug.Log("Respawn2 ativado");
        }
    }
}