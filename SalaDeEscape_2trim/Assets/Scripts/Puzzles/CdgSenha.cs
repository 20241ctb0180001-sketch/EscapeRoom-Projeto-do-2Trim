using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using FMODUnity;
using FMOD.Studio;

public class CdgSenha : MonoBehaviour
{

    private Animator anim;

    public InputActionAsset inputAction;
    private InputAction topacoEs;
    public GameObject eInteragir;
    private bool emAlcance = false;
    public bool InteractAlcance;

    //public string acerto = "event:/caixaSons/Correto";
    public string erro = "event:/caixaSons/Errado";
    public string abrindo = "event:/caixaSons/Abrir";

    [SerializeField] private TextMeshProUGUI cdgText;
    string cdgValor = "";
    private string senha = "111295";
    private bool senhaResolvida = false;
    public GameObject painelCdg;

    public GerentUI bob;

    public FirstPersonLook look;

    void Awake()
    {
        eInteragir.SetActive(false);
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
        if (cdgValor == senha && !senhaResolvida)
        {
            senhaResolvida = true;
            PlayAbrirAudio();
            anim.SetTrigger("abrirTampa");
            painelCdg.SetActive(false);
        }

        if (cdgValor.Length >= 7)
        {
            PlayErradoAudio();
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

        //bob.SetBoxInteract(emAlcance);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            eInteragir.SetActive(true);
            emAlcance = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        eInteragir.SetActive(false);
        emAlcance = false;
        painelCdg.SetActive(false);
    }

    public void AddDigit(string digito)
    {
        Debug.Log("Botão clicado! Dígito recebido: " + digito);
        cdgValor += digito;
    }

    /*void PlayCorretoAudio() => RuntimeManager.PlayOneShot(acerto, transform.position);*/
    void PlayErradoAudio() => RuntimeManager.PlayOneShot(erro, transform.position);
    void PlayAbrirAudio() => RuntimeManager.PlayOneShot(abrindo, transform.position);
}
