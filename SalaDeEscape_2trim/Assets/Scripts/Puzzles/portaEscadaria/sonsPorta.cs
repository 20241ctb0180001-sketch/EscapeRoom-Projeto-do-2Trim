using UnityEngine;

public class sonsPorta : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string fmodEventAbrir = "event:/NomeDoSeuEvento";
    public string fmodEventFechar = "event:/NomeDoSeuEvento";

    public void TocarPortaAbrindo()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventAbrir, transform.position);
    }

    public void TocarPortaFechando()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventFechar, transform.position);
    }
}
