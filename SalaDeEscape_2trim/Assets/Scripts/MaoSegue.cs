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
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
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
            Destroy(this.gameObject);
            playerSan --;
        }
        //rb.useGravity = !rb.useGravity;
        switch (playerSan)
        {
            case 1:
            painelFade.alpha = 0.9f;
            target.transform.position = limit.currRespawn.transform.position;
            painelFade.alpha = 0f;
            break;

            case 2:
            painelFade.alpha = 0.5f;
            break;

            case 3:
            painelFade.alpha = 0.25f;
            break;
        }
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