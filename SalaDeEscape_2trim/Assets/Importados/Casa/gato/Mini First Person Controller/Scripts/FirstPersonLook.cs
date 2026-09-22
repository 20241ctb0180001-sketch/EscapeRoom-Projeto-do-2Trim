using UnityEngine;
using UnityEngine.InputSystem;
public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] private Transform character;
    
    [Header("Sensibilidade Mobile / PC")]
    [Tooltip("No telemóvel, tente valores entre 0.05 e 0.2")]
    public float sensitivity = 0.1f;

    [Header("Input System")]
    public InputActionAsset InputActions;
    private InputAction cameraAction;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool jaInicializou = false;

    void Reset()
    {
        if (transform.parent != null)
            character = transform.parent;
    }

    void Start()
    {
        cameraAction = InputSystem.actions.FindAction("giroCamera");

        if (cameraAction != null)
            cameraAction.Enable();

        if (character == null && transform.parent != null)
            character = transform.parent;

        SalvarRotacaoAtual();
        jaInicializou = true; // ADICIONADO
    }

    public void SalvarRotacaoAtual()
    {
        Vector3 currentCamAngles = transform.localEulerAngles;
        xRotation = currentCamAngles.x > 180 ? currentCamAngles.x - 360 : currentCamAngles.x;

        if (character != null)
        {
            yRotation = character.eulerAngles.y;
        }

        Debug.Log($"[LOOK] SalvarRotacaoAtual chamado! xRotation={xRotation}", this);
    }

    void OnEnable()
    {
        Debug.Log("[LOOK] OnEnable disparou!", this);
        SalvarRotacaoAtual();
    }

    void Update()
    {
        OlharEmVolta();
    }

    public void OlharEmVolta()
    {
        if (cameraAction == null) return;

        Vector2 inputDelta = cameraAction.ReadValue<Vector2>();

        if (inputDelta.sqrMagnitude > 0.001f)
        {
            // O delta do Touchscreen no mobile vem em pixels brutos. 
            // Multiplicamos diretamente pela sensibilidade (SEM Time.deltaTime).
            float mouseX = inputDelta.x * sensitivity;
            float mouseY = inputDelta.y * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 85f);

            yRotation += mouseX;

            // Aplica a rotação de forma absoluta mantendo o estado
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (character != null)
            {
                character.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
    }
}