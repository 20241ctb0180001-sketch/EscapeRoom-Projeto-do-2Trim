using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GerentScena : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Jogar()
    {
        SceneManager.LoadScene("EscapeRoom");
    }
    /*public void GameOver()
    {
        SceneManager.LoadScene("");
    }*/
    public void Exit()
{
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
    Application.Quit(); // original code to quit Unity player
#endif
}
}
