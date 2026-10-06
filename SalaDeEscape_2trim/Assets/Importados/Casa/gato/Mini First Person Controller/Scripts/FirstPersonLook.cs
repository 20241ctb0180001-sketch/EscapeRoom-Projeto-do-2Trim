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

    [Header("Input System")]
    public InputActionAsset InputActions;
    private InputAction cameraAction;

    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Rotação por botão / analógico")]
    public float velocidadeBotao = 90f; // graus por segundo

    void Reset()
    {
        if (transform.parent != null)
            character = transform.parent;
    }

    void Awake()
    {
        if (character == null && transform.parent != null)
            character = transform.parent;
    }

    void Start()
    {
        cameraAction = InputSystem.actions.FindAction("giroCamera");

        if (cameraAction != null)
            cameraAction.Enable();

        //SalvarRotacaoAtual();
    }

    void Update()
    {
        OlharEmVolta();
        SalvarRotacaoAtual();
    }

    void OnEnable()
    {
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
        {
            float currentY = character.eulerAngles.y;
            yRotation = currentY > 180f ? currentY - 360f : currentY;
        }
    }

    public void OlharEmVolta()
    {
        if (cameraAction == null) return;

        Vector2 input = cameraAction.ReadValue<Vector2>();
        InputDevice device = cameraAction.activeControl?.device;

        bool viaMouse = device is Mouse;
        bool viaPonteiro = device is Pointer; // mouse ou touch: valor já é um delta

        // Cursor e bloqueio do botão direito valem só para o MOUSE
        if (!Application.isMobilePlatform && Mouse.current != null)
        {
            bool direito = Mouse.current.rightButton.isPressed;
            Cursor.lockState = direito ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !direito;

            if (viaMouse && !direito) return; // mouse sem botão direito não gira
        }

        if (input.sqrMagnitude < 0.001f) return;

        float sens;
        if (viaPonteiro)
            sens = Application.isMobilePlatform ? mobileSensitivity : pcSensitivity;
        else
            sens = velocidadeBotao * Time.deltaTime; // teclas / gamepad

        float x = input.x * sens * (inverterEixoX ? -1f : 1f);
        float y = input.y * sens * (inverterEixoY ? -1f : 1f);

        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        yRotation += x;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (character != null)
            character.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}