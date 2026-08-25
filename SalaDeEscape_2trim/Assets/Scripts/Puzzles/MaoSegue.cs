/*using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MaoSegue : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField, Range(0f, 1f)] private float aumentoAlpha = 0.67f;
    [SerializeField, Range(0f, 1f)] private float alphaParaTeleportar = 0.6f;
    [SerializeField] private GameObject particulaAoAparecer;
    [SerializeField] private EventReference somAoAparecer;
    [SerializeField] private Transform target;
    [SerializeField] private Image painelEscurece;
    [SerializeField] private Transform posicaoTeleporte;

    private Rigidbody rb;
    private bool processandoToque;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("MaoSegue: nenhum objeto com a tag Player foi encontrado.", this);
            enabled = false;
            return;
        }

        target = player.transform;

        GameObject painel = GameObject.FindGameObjectWithTag("PainelEscurece");
        if (painel != null)
            painelEscurece = painel.GetComponent<Image>();

        GameObject respawn = GameObject.FindGameObjectWithTag("respawn1");
        if (respawn != null)
            posicaoTeleporte = respawn.transform;

        if (particulaAoAparecer != null)
        {
            GameObject particula = Instantiate(particulaAoAparecer, transform.position, transform.rotation);
            Destroy(particula, 1.5f);
        }

        if (!somAoAparecer.IsNull)
            RuntimeManager.PlayOneShot(somAoAparecer, transform.position);
    }

    private void FixedUpdate()
    {
        if (target == null || rb == null || processandoToque)
            return;

        Vector3 direction = target.position - rb.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        direction.Normalize();
        rb.linearVelocity = direction * speed;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (processandoToque || !IsPlayer(other))
            return;

        processandoToque = true;

        StartCoroutine(ProcessarToque());
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.transform.root.CompareTag("Player");
    }

    private IEnumerator ProcessarToque()
    {
        float alphaAtual = painelEscurece.color.a;
        float proximoAlpha = Mathf.Clamp01(alphaAtual + aumentoAlpha);
        bool deveTeleportar = proximoAlpha >= alphaParaTeleportar;

        Debug.Log($"Alpha atual: {alphaAtual} | Próximo alpha: {proximoAlpha}");

        yield return FadeTo(proximoAlpha);

        if (deveTeleportar)
        {
            Debug.Log("Teleportando para: " + posicaoTeleporte.position);

            target.SetPositionAndRotation(
                posicaoTeleporte.position,
                posicaoTeleporte.rotation
            );

            SetAlpha(0f);
        }

        Destroy(gameObject);
    }

    private IEnumerator FadeTo(float alpha)
    {
        if (painelEscurece == null)
            yield break;

        Color startColor = painelEscurece.color;
        Color endColor = startColor;
        endColor.a = Mathf.Clamp01(alpha);
        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            painelEscurece.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        painelEscurece.color = endColor;
    }

    private void SetAlpha(float alpha)
    {
        Color color = painelEscurece.color;
        color.a = alpha;
        painelEscurece.color = color;
    }
}*/

using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MaoSegue : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField, Range(0f, 1f)] private float aumentoAlpha = 0.67f;
    [SerializeField, Range(0f, 1f)] private float alphaParaTeleportar = 0.6f;
    [SerializeField] private GameObject particulaAoAparecer;
    [SerializeField] private EventReference somAoAparecer;
    [SerializeField] private Transform target;
    [SerializeField] private Image painelEscurece;
    [SerializeField] private Transform posicaoTeleporte;

    private Rigidbody rb;
    private bool processandoToque;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("MaoSegue: nenhum objeto com a tag Player foi encontrado.", this);
            enabled = false;
            return;
        }

        target = player.transform;

        GameObject painel = GameObject.FindGameObjectWithTag("PainelEscurece");
        if (painel != null)
            painelEscurece = painel.GetComponent<Image>();

        GameObject respawn = GameObject.FindGameObjectWithTag("respawn1");
        if (respawn != null)
            posicaoTeleporte = respawn.transform;

        if (particulaAoAparecer != null)
        {
            GameObject particula = Instantiate(particulaAoAparecer, transform.position, transform.rotation);
            Destroy(particula, 1.5f);
        }

        if (!somAoAparecer.IsNull)
            RuntimeManager.PlayOneShot(somAoAparecer, transform.position);
    }

    private void FixedUpdate()
    {
        if (target == null || rb == null || processandoToque)
            return;

        Vector3 direction = target.position - rb.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        direction.Normalize();
        rb.linearVelocity = direction * speed;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (processandoToque || !IsPlayer(other))
            return;

        processandoToque = true;

        StartCoroutine(ProcessarToque());
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.transform.root.CompareTag("Player");
    }

    private IEnumerator ProcessarToque()
    {
        rb.linearVelocity = Vector3.zero;
        Vector3 posicaoInicial = rb.position;
        Vector3 posicaoFinal = posicaoInicial + Vector3.up;
        const float duracaoSubida = 2f;
        float tempo = 0f;

        while (tempo < duracaoSubida)
        {
            tempo += Time.fixedDeltaTime;
            float progresso = Mathf.Clamp01(tempo / duracaoSubida);
            rb.MovePosition(Vector3.Lerp(posicaoInicial, posicaoFinal, progresso));
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }

    private IEnumerator FadeTo(float alpha)
    {
        if (painelEscurece == null)
            yield break;

        Color startColor = painelEscurece.color;
        Color endColor = startColor;
        endColor.a = Mathf.Clamp01(alpha);
        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            painelEscurece.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        painelEscurece.color = endColor;
    }

    private void SetAlpha(float alpha)
    {
        Color color = painelEscurece.color;
        color.a = alpha;
        painelEscurece.color = color;
    }
}