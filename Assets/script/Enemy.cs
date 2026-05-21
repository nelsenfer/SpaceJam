using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum BehaviorType { Stationary, Patrol }

    [Header("Behavior Settings")]
    public BehaviorType behavior = BehaviorType.Stationary;
    public float patrolSpeed = 2f;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Detection")]
    public float closeDetectionRange = 5f; 
    public float lightDetectionRange = 15f; 
    public float lightConeAngle = 90f; 
    LayerMask wallLayer;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float aimDelay = 1f; 
    public float fireCooldown = 3f; 

    [Header("Death Settings")]
    public Sprite deadSprite; 
    public float knockbackDuration = 0.15f; 
    public float destroyDelay = 1.5f; 

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    Transform playerAimPivot;
    bool isDead = false;
    
    bool isAiming = false;
    float actionTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallLayer = LayerMask.GetMask("Wall");

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) 
        {
            player = p.transform;
            playerAimPivot = p.transform.Find("AimPivot");
            
            if (playerAimPivot == null) 
            {
                playerAimPivot = p.transform; 
            }
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        bool closeRange = IsPlayerInCloseRange();
        bool inLight = IsInLightCone();
        Vector2 direction = Vector2.zero;

        if (closeRange || inLight)
        {
            direction = (player.position - transform.position).normalized;

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
                    Shoot(player.position);
                    actionTimer = fireCooldown; 
                }
            }
        }
        else
        {
            isAiming = false; 

            if (behavior == BehaviorType.Patrol && waypoints.Length > 0)
            {
                Transform targetWaypoint = waypoints[currentWaypointIndex];
                direction = (targetWaypoint.position - transform.position).normalized;
                Patrol(targetWaypoint);
            }
        }

        UpdateAnimation(direction);
    }

    void UpdateAnimation(Vector2 dir)
    {
        if (anim != null)
        {
            if (dir != Vector2.zero)
            {
                anim.SetFloat("LookX", dir.x);
                anim.SetFloat("LookY", dir.y);
                anim.SetFloat("Speed", 1f); 
            }
            else
            {
                anim.SetFloat("Speed", 0f); 
            }
        }
    }

    void Patrol(Transform targetWaypoint)
    {
        Vector2 targetPos = targetWaypoint.position;
        Vector2 currentPos = transform.position;

        transform.position = Vector2.MoveTowards(currentPos, targetPos, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    bool IsPlayerInCloseRange()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > closeDetectionRange) return false;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distance, wallLayer);
        return hit.collider == null; 
    }

    bool IsInLightCone()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > lightDetectionRange) return false;

        Vector2 dirFromPlayer = (transform.position - playerAimPivot.position).normalized;
        float angleToEnemy = Vector2.Angle(playerAimPivot.up, dirFromPlayer);

        if (angleToEnemy <= lightConeAngle / 2f)
        {
            RaycastHit2D hit = Physics2D.Raycast(playerAimPivot.position, dirFromPlayer, distance, wallLayer);
            return hit.collider == null; 
        }

        return false;
    }

    void Shoot(Vector2 targetPos)
    {
        if (bulletPrefab != null && firePoint != null || GameManager.instance.isShooting == true)
        {
            Vector2 lookDir = targetPos - (Vector2)firePoint.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion bulletRot = Quaternion.Euler(0, 0, angle);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.shoot);
            
            Instantiate(bulletPrefab, firePoint.position, bulletRot);
        }
    }

    public void DieAndKnockback(Vector2 direction, float force)
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DeathRoutine(direction, force));
    }

    IEnumerator DeathRoutine(Vector2 direction, float force)
    {
        if (anim != null) anim.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && deadSprite != null)
        {
            sr.sprite = deadSprite;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; 
        }

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}