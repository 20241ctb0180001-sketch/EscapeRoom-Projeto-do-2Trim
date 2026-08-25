using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class terminarFase : MonoBehaviour
{
    private InputAction topacoEs;
    private bool emAlcance = false;
    public bool InteractAlcance;
    public GameObject eInteragir;
    bool podeAcabar = false;

    void Awake()
    {
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
        if (toys == 5)
        {
            podeAcabar = true;
        }
    }
}

