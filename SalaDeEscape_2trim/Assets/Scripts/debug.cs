using UnityEngine;
using UnityEngine.EventSystems;

public class debug : MonoBehaviour
{
    void Update()
    {
        // Usamos UnityEngine.InputSystem diretamente para evitar o erro de namespace
        if (UnityEngine.InputSystem.Mouse.current != null && 
            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                GameObject objetoClicado = EventSystem.current.currentSelectedGameObject;
                
                if (objetoClicado != null)
                {
                    Debug.Log("O clique acertou a UI! Objeto: " + objetoClicado.name);
                }
                else
                {
                    Debug.Log("O clique pegou num Canvas/Fundo com Raycast Target ativado (bloqueando o botão)!");
                }
            }
            else
            {
                Debug.Log("O clique NÃO acertou a UI (está indo no mundo 3D ou o EventSystem não pegou)!");
            }
        }
}
}
