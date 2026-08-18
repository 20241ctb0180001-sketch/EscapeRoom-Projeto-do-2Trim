using UnityEngine;
using UnityEngine.Events;
public class Interactables : MonoBehaviour
{
    public Item item;
    public UnityEvent OnInteract;
    public UnityEvent CollectItem;
    public bool IsMoving;

    public PreviousItem[] PreviousItem;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public void StoreOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void RestoreOriginalTransform()
    {
        // Opcional: aplica imediatamente
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    public Vector3 GetOriginalPosition() => originalPosition;
    public Quaternion GetOriginalRotation() => originalRotation;

}

[System.Serializable]
public class PreviousItem
{
    public Item requiredItem;
    public Item interactionItem;
    public UnityEvent OnInteract;
}
