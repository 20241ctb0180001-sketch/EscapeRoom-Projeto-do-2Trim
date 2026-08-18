using UnityEngine;
using UnityEngine.Events;

public class PainelInteract : MonoBehaviour
{

    [Header("Evento de Interação (Abrir Painel)")]
    public UnityEvent InteractEvent;
    public PlayerInventory inventoryPlayer;

    public void Interact()
    {
        BloqueioDeItem bloqueio = GetComponent<BloqueioDeItem>();
        if (bloqueio != null && !bloqueio.PodeInteragir(inventoryPlayer))
        {
            GerentUI.instance.ShowMessage(bloqueio.MensagemBloqueado);
            return;
        }

        InteractEvent?.Invoke();
    }
}
