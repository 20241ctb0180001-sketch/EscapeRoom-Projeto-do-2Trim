using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    public CinemachineCamera camera;
    public float defaultFOV;
    public float maxZoomFOV;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity;

    public InputActionAsset inputActions;
    private InputAction ScrollMAction;

    void Awake()
    {
        ScrollMAction = InputSystem.actions.FindAction("ScrollWheel");
        // Get the camera on this gameObject and the defaultZoom.
        camera = GetComponent<CinemachineCamera>();
        if (camera)
        {
            defaultFOV = camera.Lens.FieldOfView;
        }
    }
    void Update()
    {
        // Update the currentZoom and the camera's fieldOfView.
        Vector2 Scroll = ScrollMAction != null? ScrollMAction.ReadValue<Vector2>() : Vector2.zero;
        currentZoom += Scroll.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        camera.Lens.FieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}