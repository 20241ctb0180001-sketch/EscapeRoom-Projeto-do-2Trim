using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.Mathematics;
public class PlayerInteract : MonoBehaviour
{
    public float RayDistance;
    [SerializeField] private Camera Mycam;
    public Transform objViewer;
    public UnityEvent OnView;
    public UnityEvent OnFinishView;
    public InputActionAsset inputAction;
    private InputAction IM;
    private InputAction RotateOb;
    private Interactables CurrInteractable;
    private bool estaaVer;
    private bool canFinish;
    public float rotatSpeed;
    private Vector3 OriginPos;
    private Quaternion OiginRotat;
    private PlayerInventory inventory;

    void Awake()
    {
        Mycam = Camera.main;
        IM = InputSystem.actions.FindAction("InteractMouse");
        RotateOb = InputSystem.actions.FindAction("Look");
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        CheckInteractables();
    }

    void CheckInteractables()
    {
        if (estaaVer == true)
        {
            if (CurrInteractable == null)
            {
                estaaVer = false;
                return;
            }

            if (CurrInteractable.GetComponent<Collider>() != null)
                CurrInteractable.GetComponent<Collider>().enabled = false;

            Vector3 targetPos = objViewer.position;
            CurrInteractable.transform.position = targetPos;

            if (CurrInteractable.item.pegavel && Mouse.current.leftButton.isPressed)
            {
                RodaObj();
            }
            if (canFinish && Mouse.current.rightButton.isPressed)
            {
                FinishView();
            }
            return;
        }

        RaycastHit hit;
        Vector3 rayOrigin = Mycam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));

        if (Physics.Raycast(rayOrigin, Mycam.transform.forward, out hit, RayDistance))
            {
                PainelInteract painel = hit.collider.GetComponent<PainelInteract>();
            if (painel != null)
            {
                if (painelManager.instance != null && painelManager.instance.puzzleAtivo)
                {
                    return;
                }
                GerentUI.instance.SetPawCursor(true);
                if (IM.WasPressedThisFrame())
                {
                    painel.Interact();
                }
                return;
            }

            Interactables interactable = hit.collider.GetComponent<Interactables>();

            if (interactable != null)
            {
                GerentUI.instance.SetPawCursor(true);
                if (IM.WasPressedThisFrame())
                {
                    if (interactable.IsMoving)
                    {
                        return;
                    }
                    CurrInteractable = interactable;

                    bool hasPreviousItem = false;
                for (int i = 0; i < CurrInteractable.PreviousItem.Length; i++)
                {
                    if (inventory.itens.Contains(CurrInteractable.PreviousItem[i].requiredItem))
                    {
                        Interact(CurrInteractable.PreviousItem[i].requiredItem);
                        CurrInteractable.PreviousItem[i].OnInteract.Invoke();
                        hasPreviousItem = true;
                        break;
                    }
                }
                if (hasPreviousItem)
                {
                    return;
                }

                CurrInteractable.OnInteract.Invoke();
                if (CurrInteractable.item != null)
                {
                    OnView.Invoke();
                    estaaVer = true;
                    Invoke("CanFinish", 1f);
                    if (CurrInteractable.item.pegavel)
                    {
                        OriginPos = CurrInteractable.transform.position;
                        OiginRotat = CurrInteractable.transform.rotation;
                        StartCoroutine(MovendObj(CurrInteractable, objViewer.position));
                    }
                }
            }
        }
        else { GerentUI.instance.SetPawCursor(false); }
    }
    else { GerentUI.instance.SetPawCursor(false); }
}

    void CanFinish()
    {
        canFinish = true;
        if(CurrInteractable.item.image == null && !CurrInteractable.item.pegavel)
        {
            FinishView();
        }
        else
        {
            GerentUI.instance.SetbackImg(true);
        }
    }

    void Interact(Item item)
    {
        if(item.image != null)
        {
            GerentUI.instance.SetIntIMG(item.image);
        }
    }
    void FinishView()
    {
        canFinish = false;
        estaaVer = false;
        GerentUI.instance.SetbackImg(false);

        if (CurrInteractable.item.InvetoryItem)
        {
            inventory.AddItem(CurrInteractable.item);
            CurrInteractable.CollectItem.Invoke();
        }
        if (CurrInteractable.item.pegavel)
        {
            CurrInteractable.transform.rotation = OiginRotat;
            if (CurrInteractable.GetComponent<Collider>() != null)
            {
                CurrInteractable.GetComponent<Collider>().enabled = true;
            }
            StartCoroutine(MovendObj(CurrInteractable, OriginPos));
        }
        OnFinishView.Invoke();
    }
    IEnumerator MovendObj(Interactables obj, Vector3 pos)
    {
        obj.IsMoving = true;
        float timer = 0f;
        while (timer < 2f) //<--tava >
        {
            /*obj.transform.position = Vector3.Lerp(OriginPos, pos, timer);
            obj.transform.rotation = Quaternion.Lerp(OiginRotat, objViewer.rotation, timer);*/
            obj.transform.position = Vector3.Lerp(OriginPos, pos, timer / 2f);
            obj.transform.rotation = Quaternion.Lerp(OiginRotat, objViewer.rotation, timer / 2f);
            timer += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = pos;
        //obj.transform.rotation = objViewer.rotation;
        obj.transform.rotation = objViewer.rotation;
        obj.IsMoving = false;
    }

    void RodaObj()
    {
        float x = RotateOb.ReadValue<Vector2>().x;
        float y = RotateOb.ReadValue<Vector2>().y;
        CurrInteractable.transform.Rotate(Mycam.transform.right, Mathf.Deg2Rad * y * rotatSpeed, Space.World);
        CurrInteractable.transform.Rotate(Mycam.transform.up, -Mathf.Deg2Rad * x * rotatSpeed, Space.World);
    }


}