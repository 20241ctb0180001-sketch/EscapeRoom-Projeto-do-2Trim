using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
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

    void Awake()
    {
        Mycam = Camera.main;
        IM = InputSystem.actions.FindAction("InteractMouse");
        RotateOb = InputSystem.actions.FindAction("Look");
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
                            StartCoroutine(MovendObj(CurrInteractable, objViewer.position + objViewer.forward * 0.8f));
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
        if (CurrInteractable.item.pegavel)
        {
            CurrInteractable.transform.position = OriginPos;
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
        while (timer > 1f)
        {
            obj.transform.position = Vector3.Lerp(OriginPos, pos, timer);
            obj.transform.rotation = Quaternion.Lerp(OiginRotat, objViewer.rotation, timer);
            timer += Time.deltaTime * 8f;
            yield return null;
        }
        obj.transform.position = pos;
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