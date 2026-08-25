using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class CdgSenha : MonoBehaviour
{

    private Animator anim;

    public InputActionAsset inputAction;
    private InputAction topacoEs;

    private bool emAlcance = false;
    public bool InteractAlcance;

    [SerializeField] private TextMeshProUGUI cdgText;
    string cdgValor = "";
    private string senha = "111295";
    public GameObject painelCdg;

    public GerentUI bob;

    public FirstPersonLook look;

    void Awake()
    {
        topacoEs = InputSystem.actions.FindAction("InteractE");
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        cdgText.text = cdgValor;
        InteractAlcance = emAlcance;
        if (cdgValor == senha)
        {
            anim.SetTrigger("abrirTampa");
            painelCdg.SetActive(false);
        }

        if (cdgValor.Length >= 7)
        {
            cdgValor = "";
        }

        if (topacoEs.WasPressedThisFrame() && emAlcance == true)
        {
            painelCdg.SetActive(true);
        }

        // Só mexe no cursor/câmera se NENHUM outro puzzle estiver ativo,
        // ou se a keypad dela mesma estiver aberta
        bool outroPuzzleAtivo = painelManager.instance != null && painelManager.instance.puzzleAtivo;

        if (painelCdg.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (look != null) look.enabled = false;
        }
        else if (!outroPuzzleAtivo)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (look != null) look.enabled = true;
        }

        bob.SetBoxInteract(emAlcance);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            emAlcance = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        emAlcance = false;
        painelCdg.SetActive(false);
    }

    public void AddDigit(string digito)
    {
        Debug.Log("Botão clicado! Dígito recebido: " + digito);
        cdgValor += digito;
    }
}
