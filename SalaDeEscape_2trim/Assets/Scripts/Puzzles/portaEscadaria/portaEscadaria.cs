using UnityEngine;

public class portaEscadaria : MonoBehaviour
{
    int numerinho = 0;
    bool fecharPorta = false;
    public Animator portaBranca;
    string parametroI = "abrirPorta";
    string parametroII = "fecharPorta";
    string parametroIII = "aberto";
    bool aberto = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fecharPorta = true;
            if(fecharPorta == true && aberto == true && numerinho >=1)
            {
                portaBranca.SetBool(parametroII, true);
                portaBranca.Play("fecharPorta");
                aberto = false;
                fecharPorta = false;
                portaBranca.SetBool(parametroII, false);
            }
        }
    }

    public void tremPego(bool pego)
    {
        if(pego == true)
        {
            numerinho++;
            if(numerinho == 1)
            {
                portaBranca.SetBool(parametroI, true);
                portaBranca.Play("abrirPorta");
                aberto = true;
                portaBranca.SetBool(parametroIII, true);
                portaBranca.SetBool(parametroI, false);
            }
        }
    }
}
