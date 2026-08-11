using UnityEngine;
using UnityEngine.Events;
public class Interactables : MonoBehaviour
{
    public Item item;
    public UnityEvent OnInteract;
    public UnityEvent CollectItem;
    public bool IsMoving;
}
