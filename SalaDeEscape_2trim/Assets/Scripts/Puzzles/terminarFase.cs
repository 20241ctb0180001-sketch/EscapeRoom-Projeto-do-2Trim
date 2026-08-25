using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using FMODUnity;

public class terminarFase : MonoBehaviour
{
    private InputAction topacoEs;
    private bool emAlcance = false;
    public bool InteractAlcance;
    public GameObject eInteragir;
    public GameObject brinquedosFinais;
    bool podeAcabar = false;
    int quantia = 0;
    public EventReference somFinal;

    void Awake()
    {
        eInteragir.SetActive(false);
        brinquedosFinais.SetActive(false);
        topacoEs = InputSystem.actions.FindAction("InteractE");
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (topacoEs.WasPressedThisFrame() && emAlcance == true)
        {
            if(podeAcabar == true)
            {
                print("Acabou a fase");
                brinquedosFinais.SetActive(true);
                FMODUnity.RuntimeManager.PlayOneShot(somFinal, transform.position);
            }
            else
            {
                print("Ainda falta pegar todos os brinquedos");
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            emAlcance = true;
            eInteragir.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        emAlcance = false;
        eInteragir.SetActive(false);
    }

    public void todosAqui(int toys)
    {
        quantia = quantia + toys;
        if (quantia == 5)
        {
            podeAcabar = true;
        }
    }
}

