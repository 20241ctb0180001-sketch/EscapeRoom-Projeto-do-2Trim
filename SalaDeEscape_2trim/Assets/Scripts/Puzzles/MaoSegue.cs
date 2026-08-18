using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MaoSegue : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Rigidbody rb;
    public float speed;
    public float rotateSpeed;
    public int playerSan = 3;
    public Image painelEscurece;
    //public PlayerLimit limit;
    public float fadeDuration = 0.5f;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        //limit = GameObject.FindGameObjectWithTag("Limit").GetComponent<PlayerLimit>();
        painelEscurece = GameObject.FindGameObjectWithTag("PainelEscurece").GetComponent<Image>();
        
        // Inicializa painel transparente
        Color startColor = painelEscurece.color;
        startColor.a = 0f;
        painelEscurece.color = startColor;
    }

    void FixedUpdate()
    {
        Vector3 direction = target.position - rb.position;
        direction.Normalize();
        rb.linearVelocity = direction * speed;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {   
            Destroy(gameObject);
            playerSan--;
            Debug.Log("Player Sanity: " + playerSan);
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color startColor = painelEscurece.color;
        Color endColor = startColor;
        endColor.a = playerSan / 3f; // Quanto mais dano, mais escuro

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            painelEscurece.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            yield return null;
        }

        painelEscurece.color = endColor;
    }
}
