using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform aimPivot;
    public float aimLerpSpeed = 15f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.5f;

    [Header("Laser Sight & Aim")]
    public LineRenderer laserLine;
    public float laserDistance = 15f;
    public LayerMask obstacleLayer;
    public Transform redDot;

    [Header("Transition Settings")]
    public float startDelay = 2f;

    Rigidbody2D rb;
    Animator anim;
    Vector2 moveInput;
    Vector2 mousePos;
    float nextFireTime;

    InputAction moveAction;
    InputAction shootAction;

    private bool isReadyToPlay = false;
    private bool hasInteracted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        moveAction = new InputAction("Move", binding: "2DVector");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.Enable();

        shootAction = new InputAction("Shoot", binding: "<Mouse>/leftButton");
        shootAction.performed += ctx => Shoot();
        shootAction.Enable();

        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(startDelay);
        isReadyToPlay = true;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            mousePos = Camera.main.ScreenToWorldPoint(screenPos);
        }

        if (redDot != null)
        {
            redDot.position = mousePos;
        }

        Vector2 lookDir = mousePos - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        aimPivot.rotation = Quaternion.Lerp(aimPivot.rotation, targetRotation, aimLerpSpeed * Time.unscaledDeltaTime);

        DrawLaser();

        if (!isReadyToPlay)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (!hasInteracted && (moveInput.sqrMagnitude > 0 || Mouse.current.leftButton.wasPressedThisFrame))
        {
            hasInteracted = true;
        }

        if (!hasInteracted)
        {
            Time.timeScale = 1f;
        }
        else
        {
            if (moveInput.sqrMagnitude > 0)
            {
                Time.timeScale = 1f;
            }
            else if (Button_Manager.Instance.normalTime == false)
            {
                Time.timeScale = 0.03f;
            }
        }

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        UpdateAnimationDirection();
    }

    void FixedUpdate()
    {
        if (!isReadyToPlay) return;
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateAnimationDirection()
    {
        if (anim == null) return;

        Vector2 lookDir = (mousePos - (Vector2)transform.position).normalized;

        if (Mathf.Abs(lookDir.x) > Mathf.Abs(lookDir.y))
        {
            anim.SetFloat("LookX", lookDir.x > 0 ? 1f : -1f);
            anim.SetFloat("LookY", 0f);
        }
        else
        {
            anim.SetFloat("LookX", 0f);
            anim.SetFloat("LookY", lookDir.y > 0 ? 1f : -1f);
        }

        anim.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    void Shoot()
    {
        if (!isReadyToPlay || GameManager.instance.isShooting == false ) return;

        hasInteracted = true;

        if (Time.unscaledTime >= nextFireTime && bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, aimPivot.rotation);
            nextFireTime = Time.unscaledTime + fireCooldown;
        }
    }

    void DrawLaser()
    {
        if (laserLine == null || firePoint == null) return;

        laserLine.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, aimPivot.up, laserDistance, obstacleLayer);

        if (hit.collider != null)
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, firePoint.position + (Vector3)aimPivot.up * laserDistance);
        }
    }
}