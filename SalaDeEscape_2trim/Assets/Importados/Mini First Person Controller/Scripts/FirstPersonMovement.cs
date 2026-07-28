using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    //public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Input system")]
    public InputActionAsset InputActions;
    private InputAction MoveAction;
    private InputAction CorreAction;

    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
        MoveAction = InputSystem.actions.FindAction("Move");
        CorreAction = InputSystem.actions.FindAction("Sprint");
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && CorreAction.WasPressedThisFrame();//&& Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity = new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
    }
}