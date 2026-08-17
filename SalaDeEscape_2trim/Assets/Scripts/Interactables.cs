using UnityEngine;
using UnityEngine.Events;
public class Interactables : MonoBehaviour
{
    public Item item;
    public UnityEvent OnInteract;
    public UnityEvent CollectItem;
    public bool IsMoving;

    public PreviousItem[] PreviousItem;

    [Header("Bloqueio sem item")]
    public bool RequerItem; //precisa de item obrigatoriamente
    public string MensagemBloqueado = "Voce precisa de algo para abrir isso.";

}

[System.Serializable]
public class PreviousItem
{
    public Item requiredItem;
    public Item interactionItem;
    public UnityEvent OnInteract;
}
