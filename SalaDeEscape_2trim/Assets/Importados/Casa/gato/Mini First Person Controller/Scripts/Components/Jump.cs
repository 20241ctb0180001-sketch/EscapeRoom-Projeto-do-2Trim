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
        OnJump();
    }

    public void OnJump()
    {
        // Jump when the Jump button is pressed and we are on the ground.
        if (PuloAction.WasPressedThisFrame() && (!groundCheck || groundCheck.isGrounded))
        {
            RB.AddForce(Vector3.up * 100 * jumpStrength);
            Jumped?.Invoke();
        }
    }
}
