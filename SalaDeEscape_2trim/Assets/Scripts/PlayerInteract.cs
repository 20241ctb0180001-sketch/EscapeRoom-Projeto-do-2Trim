using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float RayDistance;
    [SerializeField]private Camera Mycam;

    void Awake(){ Mycam = Camera.main; }
    void Update()
    {
        CheckInteractables();
    }
    void CheckInteractables()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Mycam.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        if(Physics.Raycast(rayOrigin, Mycam.transform.forward, out hit, RayDistance))
        {
            Interactables interactable = hit.collider.GetComponent<Interactables>();
            if(interactable != null)
            {
                GerentUI.instance.SetPawCursor(true);
                Debug.Log("hit!");
            }
            else
            {
                GerentUI.instance.SetPawCursor(false);
            }
        }
        else
        {
            GerentUI.instance.SetPawCursor(false);
        }
    }
}
