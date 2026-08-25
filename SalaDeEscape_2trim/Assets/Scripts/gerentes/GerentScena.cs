using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GerentScena : MonoBehaviour
{
    public GameObject PSair;
    public GameObject Controls;
    public GameObject Confirm;
    [SerializeField] private string nomeLvGame;

    public void ReturnToMainMenu()
    {

        SceneManager.LoadScene("Menu");
        
    }

    public void Play()
    {
        
        SceneManager.LoadScene("Cutscene");
        
    }

    public void Controles()
    {
       
        Controls.SetActive(true);
        Confirm.SetActive(true);

    }

    public void ControlesSair()
    {
       
        Controls.SetActive(false);
        Confirm.SetActive(false);

    }

    public void Exit()
    {
       
        PSair.SetActive(true);
        Confirm.SetActive(true);

    }

    public void ExitNo()
    {
        
        PSair.SetActive(false);
        Confirm.SetActive(false);

    }

    public void ExitYes()
    {

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif

    }
}
