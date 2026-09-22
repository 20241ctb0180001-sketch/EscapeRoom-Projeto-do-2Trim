using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] Rigidbody RB;
    public float jumpStrength;
    public event System.Action Jumped;

    public InputActionAsset inputActions;
    private InputAction PuloAction;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    void Start()
    {
        PuloAction = InputSystem.actions.FindAction("Jump");
    }

    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        // Get rigidbody.
        RB = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        // Se a tecla/botão configurado no Input System for pressionado
        if (PuloAction != null && PuloAction.WasPressedThisFrame())
        {
            OnJump();
        }
    }

    public void OnJump()
    {
       // Verifica se está no chão antes de pular
        if (!groundCheck || groundCheck.isGrounded)
        {
            // Usamos ForceMode.Impulse para pulos instantâneos com Rigidbody
            RB.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            Jumped?.Invoke();
        }
    }
}
