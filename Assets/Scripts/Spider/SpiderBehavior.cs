using UnityEngine;

public class SpiderBehavior : MonoBehaviour
{
    [Header("Paramètres internes")]
    private Transform target;
    private float moveSpeed;
    private float stopDistance;

    private int spiderIndex;
    private int totalSpiders;

    [Header("Effets visuels")]
    public float appearDuration = 1.5f;

    private Vector3 initialScale;
    private bool isActive = false;

    /* =========================
       INITIALISATION
       ========================= */
    public void Initialize(
        Transform playerTarget,
        float speed,
        float minDistance,
        int index,
        int total)
    {
        target = playerTarget;
        moveSpeed = speed;
        stopDistance = minDistance;
        spiderIndex = index;
        totalSpiders = total;

        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;

        Invoke(nameof(ActivateSpider), index * 0.3f); // apparition progressive
    }

    private void ActivateSpider()
    {
        isActive = true;
        StartCoroutine(AppearCoroutine());
    }

    /* =========================
       UPDATE
       ========================= */
    void Update()
    {
        if (!isActive || target == null) return;

        MoveTowardsTarget();
        LookAtTarget();
    }

    /* =========================
       MOUVEMENT
       ========================= */
    private void MoveTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector3 moveDir = direction.normalized;

            // Variation légère pour éviter un mouvement trop "robot"
            float offset = Mathf.Sin(Time.time + spiderIndex) * 0.1f;
            moveDir += transform.right * offset;

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    private void LookAtTarget()
    {
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotation,
                Time.deltaTime * 3f
            );
        }
    }

    /* =========================
       APPARITION PROGRESSIVE
       ========================= */
    private System.Collections.IEnumerator AppearCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }

    /* =========================
       MISE À JOUR DISTANCE (SLIDER)
       ========================= */
    public void UpdateStopDistance(float newDistance)
    {
        stopDistance = newDistance;
    }
}
