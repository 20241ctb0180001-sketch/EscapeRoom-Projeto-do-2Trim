using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MaoSegue : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Rigidbody rb;
    public float speed;
    public float rotateSpeed;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = target.position - rb.position;
        direction.Normalize();
        rb.linearVelocity = direction * speed;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject, 2f);
        }

        rb.useGravity = !rb.useGravity;
    }

}
/*void DoFade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / 200;
           
        }
    }*/