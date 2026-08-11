using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class PlayerInteract : MonoBehaviour
{
    public float RayDistance;
    [SerializeField] private Camera Mycam;
    public Transform objViewer;
    public InputActionAsset inputAction;
    private InputAction IM;
    private Interactables InterAtual;
    private bool estaaVer;
    private Vector3 OriginPos;
    private Quaternion OiginRotat;

    void Awake()
    {
        Mycam = Camera.main;
        IM = InputSystem.actions.FindAction("InteractMouse");
    }
    void Update()
    {
        CheckInteractables();
    }
    void CheckInteractables()
    {
        RaycastHit hit;
        Vector3 rayOrigin = Mycam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        if (Physics.Raycast(rayOrigin, Mycam.transform.forward, out hit, RayDistance))
        {
            Interactables interactable = hit.collider.GetComponent<Interactables>();
            if (interactable != null)
            {
                GerentUI.instance.SetPawCursor(true);
                if (IM.WasPressedThisFrame())
                {
                    InterAtual = interactable;
                    estaaVer = true;
                    if (InterAtual.item.pegavel)
                    {
                        OriginPos = InterAtual.transform.position;
                        OiginRotat = InterAtual.transform.rotation;
                        StartCoroutine(MovendObj(InterAtual, objViewer.position));
                    }
                }
            }
            else
            {
                GerentUI.instance.SetPawCursor(false);
            }
        }
        else
        {
            GerentUI.instance.SetPawCursor(false);
        }
    }

    IEnumerator MovendObj(Interactables obj, Vector3 pos)
    {
        float timer = 0;
        while(timer > 1)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, pos, Time.deltaTime * 5);
            timer += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = pos;

    }
}
