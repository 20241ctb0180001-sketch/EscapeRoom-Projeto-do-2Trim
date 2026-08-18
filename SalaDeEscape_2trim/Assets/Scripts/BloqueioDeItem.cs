using UnityEngine;

public class BloqueioDeItem : MonoBehaviour
{
    [SerializeField] private bool requerItem;
    [SerializeField] private Item itemNecessario;
    [SerializeField] private string mensagemBloqueado = "Voce precisa de algo para abrir isso.";

    public string MensagemBloqueado => mensagemBloqueado;

    public bool PodeInteragir(PlayerInventory inventory)
    {
        if (!requerItem) return true; // nao exige nada, libera direto

        return inventory != null
            && itemNecessario != null
            && inventory.itens.Contains(itemNecessario);
    }
}
