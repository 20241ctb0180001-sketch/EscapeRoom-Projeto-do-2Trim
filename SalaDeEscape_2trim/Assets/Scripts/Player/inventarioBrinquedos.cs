using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class inventarioBrinquedos : MonoBehaviour
{
    private InputAction abrirInventario;
    public GameObject brinquedoInventario;
    bool inventarioLig = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abrirInventario = InputSystem.actions.FindAction("abrirInventarioBrinquedos");
        // gameObject.SetActive(false);
        brinquedoInventario.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inventarioLig)
        {
            gameObject.SetActive(true);
            if (abrirInventario.WasReleasedThisFrame())
            {
                MostrarInventario();
            }
        }
    }

    public void MostrarInventario()
    {
        if (brinquedoInventario.activeInHierarchy)
        {
            brinquedoInventario.SetActive(false);
        }else
        {
            brinquedoInventario.SetActive(true);
        }
    }

    public void ativarInventario(bool taPodendo)
    {
        if(taPodendo){
            print("AI, meu bumbum");
        }
        inventarioLig = taPodendo;
    }
}
