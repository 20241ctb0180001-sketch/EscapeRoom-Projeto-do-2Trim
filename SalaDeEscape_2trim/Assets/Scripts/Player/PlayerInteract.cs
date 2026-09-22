using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInteract : MonoBehaviour
{
    public GameObject BInv;
    public GameObject portinha;
    public GameObject bricador;
    [SerializeField] private float RayDistance;
    [SerializeField] private Camera Mycam;
    public Transform objViewer;
    public UnityEvent OnView;
    public UnityEvent OnFinishView;
    public InputActionAsset inputAction;
    private InputAction IMinterage;
    private InputAction IMsai;
    private InputAction RotateOb;
    private Interactables CurrInteractable;
    private bool estaaVer;
    private bool canFinish;
    [SerializeField] private float rotatSpeed = 100f; // Ajuste a velocidade na Inspector se necessário
    private Vector3 OriginPos;
    private Quaternion OiginRotat;
    private PlayerInventory inventory;

    [Header("Câmera e Movimento")]
    [SerializeField] private FirstPersonLook look;
    [SerializeField] private FirstPersonMovement movement;
    [SerializeField] private float animationDuration = 0.5f;
    private GerenciadorInventario inventario;
    private portaEscadaria abrate;
    private inventarioBrinquedos ToyInvent;

    void Awake()
    {
        Mycam = Camera.main;
        IMinterage = InputSystem.actions.FindAction("InteractMouseVe");
        IMsai = InputSystem.actions.FindAction("InteractMouseSaiVe");
        RotateOb = InputSystem.actions.FindAction("Look");
        inventory = GetComponent<PlayerInventory>();
        abrate = portinha != null ? portinha.GetComponent<portaEscadaria>() : null;
        inventario = bricador != null ? bricador.GetComponent<GerenciadorInventario>() : null;
        if (BInv != null)
        {
            ToyInvent = BInv.GetComponent<inventarioBrinquedos>();
        }
    }

    void Update()
    {
        CheckInteractables();
    }

    void CheckInteractables()
    {

        void CheckInteractables()
{
    if (painelManager.instance != null && painelManager.instance.puzzleAtivo)
    {
        GerentUI.instance.SetPawCursor(false);
        return;
    }

    if (estaaVer)
    {
        GerentUI.instance.SetPawCursor(false);

        if (CurrInteractable == null)
        {
            estaaVer = false;
            return;
        }

        if (CurrInteractable.GetComponent<Collider>() != null)
            CurrInteractable.GetComponent<Collider>().enabled = false;

        if (!CurrInteractable.IsMoving)
        {
            CurrInteractable.transform.position = objViewer.position;
        }

        // Executa a rotação do objeto
        interag();

        // Só tenta verificar saída se a flag canFinish estiver ativa
        if (canFinish)
        {
            saiInterag();
        }

        return;
    }
}

        // --- MODO NORMAL (RAYCAST) ---
        RaycastHit hit;
        Vector3 rayOrigin = Mycam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)); // Centro da tela

        if (Physics.Raycast(rayOrigin, Mycam.transform.forward, out hit, RayDistance))
        {
            PainelInteract painel = hit.collider.GetComponent<PainelInteract>();
            if (painel != null)
            {
                if (painelManager.instance != null && painelManager.instance.puzzleConcluido)
                {
                    GerentUI.instance.SetPawCursor(false);
                    return;
                }

                GerentUI.instance.SetPawCursor(true);
                if (IMinterage.WasPressedThisFrame())
                {
                    painel.Interact();
                }
                return;
            }

            Interactables interactable = hit.collider.GetComponent<Interactables>();

            if (interactable != null)
            {
                GerentUI.instance.SetPawCursor(true);
                if (IMinterage.WasPressedThisFrame())
                {
                    if (interactable.IsMoving) return;

                    CurrInteractable = interactable;

                    if (interactable.CompareTag("brinquedos"))
                    {
                        ColetarBrinquedoDireto(interactable);
                        return;
                    }

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
                    if (hasPreviousItem) return;

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

                            // DESATIVA A PATA AO ENTRAR NA INSPEÇÃO
                            GerentUI.instance.SetPawCursor(false); 

                            if (look != null) look.enabled = false;
                            if (movement != null) movement.enabled = false;
                        
                        Invoke("CanFinish", 0.5f);
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

    public void interag()
    {
        // Roda o objeto continuamente se for pegável
        if (CurrInteractable != null && CurrInteractable.item != null && CurrInteractable.item.pegavel)
        {
            RodaObj();
        }
    }

   public void saiInterag()
    {
        // Apenas fecha se a ação de sair for acionada (ex: tecla ESC ou um Botão de Fechar na UI)
        if (canFinish && IMsai != null && IMsai.WasPressedThisFrame())
        {
            FinishView();
            if (look != null) look.enabled = true;
            if (movement != null) movement.enabled = true;
        }
    }

    // Método PÚBLICO para você conectar diretamente a um Botão "Sair/Voltar" da UI no Mobile
    public void BotaoSairInspecao()
    {
        if (canFinish && estaaVer)
        {
            FinishView();
            if (look != null) look.enabled = true;
            if (movement != null) movement.enabled = true;
        }
    }

    private void ColetarBrinquedoDireto(Interactables interactable)
    {
        BrinquedoColetavel coletavel = interactable.GetComponent<BrinquedoColetavel>();
        if (CurrInteractable.CompareTag("brinquedos"))
        {
            if (inventario != null && coletavel != null) inventario.AdicionarItem(coletavel.dadosDoItem);
            CurrInteractable.CollectItem.Invoke();
            if (CurrInteractable.gameObject.name == "Trenzinho")
            {
                if (abrate != null) abrate.tremPego(true);
                if (ToyInvent != null) ToyInvent.ativarInventario(true);
            }
        }
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

        // Reativa a câmera e movimento
        if (look != null) look.enabled = true;
        if (movement != null) movement.enabled = true;

        GerentUI.instance.SetbackImg(false);

        BrinquedoColetavel coletavel = CurrInteractable.GetComponent<BrinquedoColetavel>();
        if (CurrInteractable.item.InvetoryItem)
        {
            if (CurrInteractable.CompareTag("brinquedos"))
            {
                if (inventario != null && coletavel != null) inventario.AdicionarItem(coletavel.dadosDoItem);
                CurrInteractable.CollectItem.Invoke();
                if (CurrInteractable.gameObject.name == "Trenzinho")
                {
                    if (abrate != null) abrate.tremPego(true);
                    if (ToyInvent != null) ToyInvent.ativarInventario(true);
                }
            }
            else
            {
                if (inventory != null) inventory.AddItem(CurrInteractable.item);
                CurrInteractable.CollectItem.Invoke();
            }
        }

        if (CurrInteractable.item.pegavel)
        {
            if (CurrInteractable.GetComponent<Collider>() != null)
            {
                CurrInteractable.GetComponent<Collider>().enabled = true;
            }

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

        obj.transform.position = targetPos;
        obj.transform.rotation = targetRot;
        obj.IsMoving = false;
    }

    void RodaObj()
    {
        if (RotateOb == null) return;

        Vector2 delta = RotateOb.ReadValue<Vector2>();

        if (delta.sqrMagnitude > 0.01f)
        {
            // Usa o delta de toque/mouse diretamente com a velocidade de rotação do objeto
            float xRot = delta.y * rotatSpeed * 0.1f;
            float yRot = -delta.x * rotatSpeed * 0.1f;

            CurrInteractable.transform.Rotate(Mycam.transform.right, xRot, Space.World);
            CurrInteractable.transform.Rotate(Mycam.transform.up, yRot, Space.World);
        }
    }
}