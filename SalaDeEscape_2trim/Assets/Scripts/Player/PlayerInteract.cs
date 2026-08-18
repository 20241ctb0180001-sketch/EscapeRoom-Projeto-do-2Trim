using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInteract : MonoBehaviour
{
    public GameObject bricador;
    [SerializeField] private float RayDistance;
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
    [SerializeField] private float rotatSpeed;
    private Vector3 OriginPos;
    private Quaternion OiginRotat;
    private PlayerInventory inventory;
    [Header("Câmera e Movimento")]
    [SerializeField] private FirstPersonLook look;
    [SerializeField] private FirstPersonMovement movement;
    [SerializeField] private float animationDuration = 2f;
    private GerenciadorInventario inventario;


    void Awake()
    {
        inventario = bricador.GetComponent<GerenciadorInventario>();
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
                if (look != null) look.enabled = false;
                if (movement != null) movement.enabled = false;
                RodaObj();
            }
            if (canFinish && Mouse.current.rightButton.isPressed)
            {
                FinishView();
                if (look != null) look.enabled = true;
                if (movement != null) movement.enabled = true;
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
                        if (inventory.Itens.Contains(CurrInteractable.PreviousItem[i].requiredItem))
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

                    BloqueioDeItem bloqueio = CurrInteractable.GetComponent<BloqueioDeItem>();
                    if (bloqueio != null && !bloqueio.PodeInteragir(inventory))
                    {
                        GerentUI.instance.ShowMessage(bloqueio.MensagemBloqueado);
                        return;
                    }

                    CurrInteractable.OnInteract.Invoke();
                    if (CurrInteractable.item != null)
                    {
                        Interact(CurrInteractable.item);
                        OnView.Invoke();
                        estaaVer = true;
                        if (look != null) look.enabled = false;
                        if (movement != null) movement.enabled = false;
                        Invoke("CanFinish", 1f);
                        if (CurrInteractable.item.pegavel)
                        {
                            OriginPos = CurrInteractable.transform.position;
                            OiginRotat = CurrInteractable.transform.rotation;

                            CurrInteractable.StoreOriginalTransform();
                            StartCoroutine(MovendObj(CurrInteractable, objViewer.position, objViewer.rotation));
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
        if (CurrInteractable.item.image == null && !CurrInteractable.item.pegavel)
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
        if (item.image != null)
        {
            GerentUI.instance.SetIntIMG(item.image);
        }
    }
    void FinishView()
    {
        canFinish = false;
        estaaVer = false;
        if (look != null) look.enabled = true;
        if (movement != null) movement.enabled = true;
        GerentUI.instance.SetbackImg(false);

        BrinquedoColetavel coletavel = CurrInteractable.GetComponent<BrinquedoColetavel>();
        if (CurrInteractable.item.InvetoryItem)
        {
            if (CurrInteractable.CompareTag("brinquedos"))
            {
                inventario.AdicionarItem(coletavel.dadosDoItem);
                CurrInteractable.CollectItem.Invoke();
            }else
            {
                inventory.AddItem(CurrInteractable.item);
                CurrInteractable.CollectItem.Invoke();
            }
            
        }
        if (CurrInteractable.item.pegavel)
        {
            CurrInteractable.transform.rotation = OiginRotat;
            if (CurrInteractable.GetComponent<Collider>() != null)
            {
                CurrInteractable.GetComponent<Collider>().enabled = true;
            }
            //StartCoroutine(MovendObj(CurrInteractable, OriginPos));

            CurrInteractable.RestoreOriginalTransform();
            StartCoroutine(MovendObj(CurrInteractable, CurrInteractable.GetOriginalPosition(), CurrInteractable.GetOriginalRotation()));
        }
        OnFinishView.Invoke();
    }

    IEnumerator MovendObj(Interactables obj, Vector3 targetPos, Quaternion targetRot)
    {
        obj.IsMoving = true;
        float timer = 0f;
        Vector3 startPos = obj.transform.position;
        Quaternion startRot = obj.transform.rotation;

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;
            obj.transform.position = Vector3.Lerp(startPos, targetPos, progress);
            obj.transform.rotation = Quaternion.Lerp(startRot, targetRot, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        // Garante valores finais exatos
        obj.transform.position = targetPos;
        obj.transform.rotation = targetRot;
        obj.IsMoving = false;
    }

    /*IEnumerator MovendObj(Interactables obj, Vector3 pos)
    {
        obj.IsMoving = true;
        float timer = 0f;
        while (timer < 2f) //<--tava >
        {
            obj.transform.position = Vector3.Lerp(OriginPos, pos, timer / 2f);
            obj.transform.rotation = Quaternion.Lerp(OiginRotat, objViewer.rotation, timer / 2f);
            timer += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = pos;
        obj.transform.rotation = objViewer.rotation;
        obj.IsMoving = false;
    }*/

    void RodaObj()
    {
        float x = RotateOb.ReadValue<Vector2>().x;
        float y = RotateOb.ReadValue<Vector2>().y;
        CurrInteractable.transform.Rotate(Mycam.transform.right, Mathf.Deg2Rad * y * rotatSpeed, Space.World);
        CurrInteractable.transform.Rotate(Mycam.transform.up, -Mathf.Deg2Rad * x * rotatSpeed, Space.World);
    }


}