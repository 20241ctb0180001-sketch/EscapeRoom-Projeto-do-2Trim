using UnityEngine;
using UnityEngine.Events;

public class PainelInteract : MonoBehaviour
{

    [Header("Evento de Interação (Abrir Painel)")]

    public UnityEvent InteractEvent; //evento "AbrirPuzzle"

    //função chamada pelo PlayerIntera quando o jogador clica no painel
    public void Interact()
    {
        InteractEvent?.Invoke(); //dispara o evento programado no Inspector
    }

}
