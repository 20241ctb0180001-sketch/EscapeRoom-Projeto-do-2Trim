using System.Collections.Generic;
using UnityEngine;

public class GerenciadorInventario : MonoBehaviour
{
    [Header("Dados")]
    // Lista fixa com os 5 itens atuais do inventário (null = slot vazio)
    public List<Item> itensNoInventario = new List<Item>(5);

    [Header("UI")]
    // Arraste no Inspector os 5 GameObjects de slot na ordem
    [SerializeField] private List<SlotInventario> slotsUI = new List<SlotInventario>(5);

    private void Start()
    {
        AtualizarTodaUI();
    }

    /// <summary>
    /// Percorre todos os slots e atualiza a UI correspondente.
    /// </summary>

    public void AtualizarTodaUI()
    {
        print("qqqqq");
        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (i < itensNoInventario.Count)
            {
                slotsUI[i].AtualizarSlot(itensNoInventario[i]);
                print("" + itensNoInventario[i]);
            }
            else
            {
                slotsUI[i].AtualizarSlot(null);
            }
        }
    }

    // Exemplo de como adicionar um item ao primeiro slot livre
    public bool AdicionarItem(Item novoItem)
    {
        for (int i = 0; i < itensNoInventario.Count; i++)
        {
            if (itensNoInventario[i] == null)
            {
                itensNoInventario[i] = novoItem;
                AtualizarTodaUI();
                return true; // Item adicionado com sucesso
            }
        }
        Debug.Log("Inventário cheio!");
        return false;
    }
}