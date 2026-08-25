using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditosRolagem : MonoBehaviour
{
    public float velocidade = 50f;
    public string nomeCenaMenu = "MenuPrincipal";
    public float limiteY = 1000f; // Posição onde a mensagem final fica no centro

    void Update()
    {
        // Se ainda não chegou no limite, continua subindo
        if (transform.localPosition.y < limiteY)
        {
            transform.Translate(Vector3.up * velocidade * Time.deltaTime);
        } else
        {
            // Chegou no limite? Volta para o menu automaticamente!
            SceneManager.LoadScene("Menu");
        }

    }
}