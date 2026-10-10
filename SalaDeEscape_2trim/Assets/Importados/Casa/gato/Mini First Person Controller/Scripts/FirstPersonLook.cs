using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] private Transform character;

    [Header("Sensibilidade")]
    public float pcSensitivity = 1.0f;
    public float mobileSensitivity = 0.1f;

    [Header("Inversão de Eixos")]
    public bool inverterEixoY = false;
    public bool inverterEixoX = false;

    [Header("Rotação por botão / analógico")]
    public float velocidadeBotao = 90f; // graus por segundo
    private Rigidbody characterRB;
    private InputAction cameraAction;
    private float xRotation;
    private float yRotation;

    // void Reset()
    // {
    //     if (transform.parent != null) character = transform.parent;
    // }
    void Awake()
    {
        // if (character == null && transform.parent != null)
        //     character = transform.parent;

        if (character != null)
            characterRB = character.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        SalvarRotacaoAtual(); // só aqui e no Start
    }

    void Start()
    {
        cameraAction = InputSystem.actions.FindAction("giroCamera");
        cameraAction?.Enable();
        SalvarRotacaoAtual();
    }

    void OnDisable()
    {
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SalvarRotacaoAtual()
    {
        float currentX = transform.localEulerAngles.x;
        xRotation = currentX > 180f ? currentX - 360f : currentX;

        if (character != null)
            yRotation = character.eulerAngles.y;
    }

    void Update()
    {
        OlharEmVolta(); // só atualiza xRotation / yRotation
    }

    void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (character != null)
        {
            Quaternion rot = Quaternion.Euler(0f, yRotation, 0f);
            character.rotation = rot;
            if (characterRB != null)
                characterRB.rotation = rot; // mantém o Rigidbody sincronizado
        }
    }

    void OlharEmVolta()
    {
        if (cameraAction == null) return;

        Vector2 input = cameraAction.ReadValue<Vector2>();
        InputDevice device = cameraAction.activeControl?.device;

        bool viaMouse = device is Mouse;
        bool viaPonteiro = device is Pointer;

        if (!Application.isMobilePlatform && Mouse.current != null)
        {
            bool direito = Mouse.current.rightButton.isPressed;
            Cursor.lockState = direito ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !direito;

            if (viaMouse && !direito) return;
        }

        if (input.sqrMagnitude < 0.001f) return;

        float sens = viaPonteiro ? (Application.isMobilePlatform ? mobileSensitivity : pcSensitivity) : velocidadeBotao * Time.deltaTime;

        float x = input.x * sens * (inverterEixoX ? -1f : 1f);
        float y = input.y * sens * (inverterEixoY ? -1f : 1f);

        xRotation = Mathf.Clamp(xRotation - y, -85f, 85f);
        yRotation += x;
    }
}