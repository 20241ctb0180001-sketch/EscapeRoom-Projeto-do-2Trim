using UnityEngine;
using UnityEngine.Events;

public class PainelInteract : MonoBehaviour
{

    [Header("Evento de Interação (Abrir Painel)")]
    public UnityEvent InteractEvent;

    [Header("Bloqueio sem item")]
    public bool RequerItem;
    public Item itemNecessario;
    public string MensagemBloqueado = "Voce precisa de algo para abrir isso.";
    public PlayerInventory inventoryPlayer;

    public void Interact()
    {
        if (RequerItem)
        {
            bool temItem = inventoryPlayer != null
                && itemNecessario != null
                && inventoryPlayer.itens.Contains(itemNecessario);

            if (!temItem)
            {
                GerentUI.instance.ShowMessage(MensagemBloqueado);
                return;
            }
        }

        InteractEvent?.Invoke();
    }
}
