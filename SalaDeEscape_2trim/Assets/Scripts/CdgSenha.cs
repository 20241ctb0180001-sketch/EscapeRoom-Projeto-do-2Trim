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

    [SerializeField] private TextMeshProUGUI cdgText;
    string cdgValor = "";
    public string senha;
    public GameObject painelCdg;

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

        if(cdgValor == senha)
        {
            anim.SetTrigger("abrirTampa");
            painelCdg.SetActive(false);
        }

        if(cdgValor.Length >= 7)
        {
            cdgValor = "";
        }

        if(topacoEs.WasPressedThisFrame() && emAlcance == true)
        {
            painelCdg.SetActive(true);
        }

        if(painelCdg.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
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
        cdgValor += digito;
    }
}
