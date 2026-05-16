using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f;
    LayerMask wallLayer;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float aimDelay = 1f;
    public float fireCooldown = 3f;

    Rigidbody2D rb;
    Transform player;
    bool isDead = false;

    bool isAiming = false;
    float actionTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallLayer = LayerMask.GetMask("Wall");

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && HasLineOfSight())
        {
            RotateTowardsPlayer();

            if (!isAiming)
            {
                isAiming = true;
                actionTimer = aimDelay;
            }
            else
            {
                actionTimer -= Time.deltaTime;
                if (actionTimer <= 0f)
                {
                    Shoot();
                    actionTimer = fireCooldown;
                }
            }
        }
        else
        {
            isAiming = false;
        }
    }

    bool HasLineOfSight()
    {
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, wallLayer);
        return hit.collider == null;
    }

    void RotateTowardsPlayer()
    {
        Vector2 lookDir = player.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        }
    }

    public void DieAndKnockback(Vector2 direction, float force)
    {
        if (isDead) return;
        isDead = true;

        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        Destroy(gameObject, 0.5f);
    }
}