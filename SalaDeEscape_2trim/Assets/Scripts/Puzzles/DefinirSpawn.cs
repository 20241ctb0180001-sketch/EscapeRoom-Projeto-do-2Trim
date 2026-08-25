using UnityEngine;

public class DefinirSpawn : MonoBehaviour
{
    private int nuRespawn;
    public GameObject Player;
    public GameObject objetoAlvo;
    private PlayerLimit alvoScript;
    void Start()
    {
        if (objetoAlvo != null)
        {
            alvoScript = objetoAlvo.GetComponent<PlayerLimit>();
            
        }
        if (gameObject.name == "Respawn1")
        {
            nuRespawn = 1;
        }else if (gameObject.name == "Respawn2")
        {
            nuRespawn = 2;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            print("Respawn Ativado");
            alvoScript.QualSpawnEsta(nuRespawn);
        }
    }
    
}