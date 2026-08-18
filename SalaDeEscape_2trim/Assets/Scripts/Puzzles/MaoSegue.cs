using UnityEngine;
using System.Collections;
using FMODUnity;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MaoSegue : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Rigidbody rb;
    public float speed;
    public float rotateSpeed;
    public int playerSan = 3;
    public GameObject painelEscurece;
    public PlayerLimit limit;
    private CanvasGroup painelFade;
    public FirstPersonMovement playerController;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        limit = GameObject.FindGameObjectWithTag("Limit").GetComponent<PlayerLimit>();
        painelEscurece = GameObject.FindGameObjectWithTag("PainelEscurece");
        painelFade = painelEscurece.GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
    {
        Vector3 direction = target.position - rb.position;
        direction.Normalize();
        rb.linearVelocity = direction * speed;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
    }

    void OnCollisionEnter(Collision other)
{
    if (other.gameObject.CompareTag("Player"))
    {
        FirstPersonMovement playerController = other.gameObject.GetComponent<FirstPersonMovement>();
        if (playerController != null)
        {
            playerController.TakeDamage(1);
        }
        Destroy(this.gameObject);
    }
}

}
