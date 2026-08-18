using UnityEngine;
using UnityEngine.UI;

public class SlotInventario : MonoBehaviour
{
    [SerializeField] private Image imagemIcone;
    [SerializeField] private Sprite iconeSlotVazio; // Opcional: imagem de fundo padrão

    // Atualiza o sprite da UI de acordo com o item recebido.
    // Passar 'null' limpa o slot.
    public void AtualizarSlot(Item item)
    {
        /*if(item != null)
        {
            print("aiai");
        }
        if(item.image != null)
        {
            print("uiui");
        }*/

        if (item != null && item.image != null)
        {
            imagemIcone.sprite = item.image;
            imagemIcone.enabled = true; // Mostra a imagem do item
        }
        else
        {
            imagemIcone.sprite = iconeSlotVazio;
            // Se não tiver imagem padrão para o slot vazio, desabilite a imagem:
            imagemIcone.enabled = (iconeSlotVazio != null);
        }
    }
}