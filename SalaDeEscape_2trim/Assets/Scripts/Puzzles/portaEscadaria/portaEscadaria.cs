using UnityEngine;

public class portaEscadaria : MonoBehaviour
{
    int numerinho = 0;
    bool abrirPorta = false;
    public Transform portaBranca;
    float velocidadeGiro = 5f;
    bool aberto = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(abrirPorta == true && aberto == false)
        {
            while(portaBranca.eulerAngles.y > 93.512f)
            {
                print("ffgh");
                portaBranca.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime);
                
            }
            
        }else if(abrirPorta == false && numerinho >=1)
        {
            velocidadeGiro = -150f;
            if(portaBranca.eulerAngles.y != -3.512f)
            {
                portaBranca.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime);

            }else if(portaBranca.eulerAngles.y <= -3.512f)
            {
                portaBranca.eulerAngles= new Vector3(0, -3.512f, 0);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            abrirPorta = false;
        }
    }

    public void tremPego(bool pego)
    {
        if(pego == true)
        {
            numerinho++;
            if(numerinho == 1)
            {
                abrirPorta = true;
            }
        }
    }
}
