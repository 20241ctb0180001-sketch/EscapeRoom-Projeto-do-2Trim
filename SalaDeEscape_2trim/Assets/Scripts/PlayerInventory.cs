using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> itens;
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
