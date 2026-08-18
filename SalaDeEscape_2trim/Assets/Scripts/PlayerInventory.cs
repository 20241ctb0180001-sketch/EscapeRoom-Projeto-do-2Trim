using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Item> itens;
    
    // Forma tradicional e segura de expor a lista para leitura
    public List<Item> Itens 
    {
        get { return itens; }
    }
    public void AddItem(Item item)
    {
        if (itens.Contains(item))
        {
            return;
        }
        GerentUI.instance.setItens(item, itens.Count);
        itens.Add(item);
    }
}
