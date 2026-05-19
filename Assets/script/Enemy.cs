using UnityEngine;

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

    Rigidbody2D rb;
    Transform player;
    Transform playerAimPivot;
    bool isDead = false;
    
    bool isAiming = false;
    float actionTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (closeRange || inLight)
        {
            RotateTowardsTarget(player.position);

            if (!isAiming)
            {
                isAiming = true;
                actionTimer = aimDelay; 
                
                if (closeRange) Debug.Log("⚠️ TERDETEKSI: Player masuk radius dekat!");
                else if (inLight) Debug.Log("🚨 TERDETEKSI: Player tersorot cahaya senter!");
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
            if (isAiming) 
            {
                Debug.Log("✅ AMAN: Target hilang dari pandangan.");
            }
            isAiming = false; 

            if (behavior == BehaviorType.Patrol)
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 targetPos = targetWaypoint.position;
        Vector2 currentPos = transform.position;

        RotateTowardsTarget(targetWaypoint.position);
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

    void RotateTowardsTarget(Vector3 targetPos)
    {
        Vector2 lookDir = targetPos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            Debug.Log("💥 Musuh Menembak!");
        }
    }

    public void DieAndKnockback(Vector2 direction, float force)
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("💀 Musuh Tewas Terkena Hit!");
        
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        Destroy(gameObject, 0.5f); 
    }
}