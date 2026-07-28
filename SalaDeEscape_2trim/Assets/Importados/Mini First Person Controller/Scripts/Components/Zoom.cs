using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    Camera camera;
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
        camera = GetComponent<Camera>();
        if (camera)
        {
            defaultFOV = camera.fieldOfView;
        }
    }

    void Update()
    {
        // Update the currentZoom and the camera's fieldOfView.
        //currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        camera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}
