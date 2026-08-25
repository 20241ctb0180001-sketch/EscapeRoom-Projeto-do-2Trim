using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Running")]
    public float speed = 5;
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;

    [Header("Input system")]
    public InputActionAsset InputActions;
    private InputAction MoveAction;
    private InputAction CorreAction;

    [Header("Wall detection")]
    public LayerMask wallMask;
    public float wallCheckDistance = 0.35f;
    public float wallRadius = 0.25f;

    Rigidbody RB;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        // Get the rigidbody on this.
        RB = GetComponent<Rigidbody>();
        MoveAction = InputSystem.actions.FindAction("Move");
        CorreAction = InputSystem.actions.FindAction("Sprint");
    }

    private Vector3 GetWallSafeVelocity(Vector3 desiredVelocity)
    {
        if (desiredVelocity == Vector3.zero)
            return Vector3.zero;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = desiredVelocity.normalized;

        if (Physics.SphereCast(origin, wallRadius, direction, out RaycastHit hit, wallCheckDistance, wallMask))
        {
            float dot = Vector3.Dot(hit.normal, direction);
            if (dot < 0f)
            {
                desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, hit.normal);
            }
        }
        return desiredVelocity;
    } // muda a velocidade para não deixar o player andando na parede, usando um raycast em esfera para detectar a parede

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && CorreAction != null && CorreAction.IsPressed();

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 movInput = MoveAction != null ? MoveAction.ReadValue<Vector2>() : Vector2.zero;

        //  {
        bool isAirborne = Mathf.Abs(RB.linearVelocity.y) > 0.05f;
        float moveMultiplier = isAirborne ? 0.55f : 1f;

        Vector3 desiredVelocity = transform.rotation * new Vector3( movInput.x * targetMovingSpeed * moveMultiplier, 0f, movInput.y * targetMovingSpeed * moveMultiplier);
        desiredVelocity = GetWallSafeVelocity(desiredVelocity);

        Vector3 currentVelocity = RB.linearVelocity;

        float smoothAmount = isAirborne ? 0.08f : 0.18f;
        Vector3 smoothedVelocity = new Vector3( Mathf.Lerp(currentVelocity.x, desiredVelocity.x, smoothAmount), currentVelocity.y, Mathf.Lerp(currentVelocity.z, desiredVelocity.z, smoothAmount));

        // Extra damping when no movement input is being pressed.
        if (movInput == Vector2.zero)
        {
            float damping = isAirborne ? 0.12f : 0.2f;
            smoothedVelocity.x = Mathf.Lerp(currentVelocity.x, 0f, damping);
            smoothedVelocity.z = Mathf.Lerp(currentVelocity.z, 0f, damping);
        }
        // Apply movement.
        RB.linearVelocity = smoothedVelocity;
        //   } // para impedir o player de ir muito rapido e "voar" quando pula

        //RB.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, RB.linearVelocity.y, targetVelocity.y); cod anterior.
    }
}