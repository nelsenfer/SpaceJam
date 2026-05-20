using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AdvancedUIAnimation : MonoBehaviour
{
    public enum AnimationType { MoveToDirection, FadeCanvas, MoveAndFade, CustomPopScale }
    public enum Direction { Left, Right, Up, Down }

    [Header("General Settings")]
    [SerializeField] private AnimationType animationType;
    public float duration = 0.5f; 
    
    [Tooltip("Beri jeda waktu (dalam detik) sebelum animasi dimulai")]
    [SerializeField] private float startDelay = 0f; 
    
    [Tooltip("Centang ini agar animasi OTOMATIS berjalan saat scene/object ini aktif")]
    [SerializeField] private bool playOnEnable = false;

    [Header("Loop Settings")]
    [Tooltip("Centang ini jika ingin animasi berjalan berulang-ulang (ping-pong)")]
    [SerializeField] private bool isLooping = false;

    [Header("Movement Settings")]
    [SerializeField] private Direction moveDirection;
    [SerializeField] private float moveDistance = 1100f; 

    [Header("Fade Settings")]
    [Range(0f, 1f)] [SerializeField] private float targetAlpha = 1f;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 targetScale = Vector3.one;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Coroutine currentAnim;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        originalPosition = rectTransform.anchoredPosition;
        originalScale = transform.localScale;
    }

    private void Start()
    {
        // Hubungan ke SceneTransitionManager diputus dari sini agar tidak NullReferenceException!
        
        // Jika TIDAK diatur otomatis jalan di awal, paksa posisinya sembunyi terlebih dahulu
        if (!playOnEnable)
        {
            InstantReset();
        }
        else
        {
            // Jika di-centang playOnEnable, jalankan animasinya sekarang
            PlayIn();
        }
    }

    private void OnEnable()
    {
        // Menjaga pemicuan ulang saat object dinonaktifkan lalu diaktifkan kembali
        if (playOnEnable && rectTransform != null)
        {
            PlayIn();
        }
    }

    public void InstantReset()
    {
        StopCurrent();
        if (animationType == AnimationType.MoveToDirection || animationType == AnimationType.MoveAndFade)
        {
            rectTransform.anchoredPosition = GetOffsetPosition();
        }
        if (animationType == AnimationType.FadeCanvas || animationType == AnimationType.MoveAndFade)
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }
        if (animationType == AnimationType.CustomPopScale)
        {
            transform.localScale = Vector3.zero;
        }
    }

    public void PlayIn()
    {
        StopCurrent();
        SetupStartPosition(); 
        currentAnim = StartCoroutine(MasterAnimationRoutine(true));
    }

    public void PlayOut()
    {
        StopCurrent();
        currentAnim = StartCoroutine(MasterAnimationRoutine(false));
    }

    private void StopCurrent()
    {
        if (currentAnim != null)
        {
            StopCoroutine(currentAnim);
            currentAnim = null;
        }
    }

    private void SetupStartPosition()
    {
        if (animationType == AnimationType.MoveToDirection || animationType == AnimationType.MoveAndFade)
        {
            rectTransform.anchoredPosition = GetOffsetPosition();
        }
        if (animationType == AnimationType.FadeCanvas || animationType == AnimationType.MoveAndFade)
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }
        if (animationType == AnimationType.CustomPopScale)
        {
            transform.localScale = Vector3.zero;
        }
    }

    private Vector2 GetOffsetPosition()
    {
        switch (moveDirection)
        {
            case Direction.Left: return originalPosition + Vector2.left * moveDistance;
            case Direction.Right: return originalPosition + Vector2.right * moveDistance;
            case Direction.Up: return originalPosition + Vector2.up * moveDistance;
            case Direction.Down: return originalPosition + Vector2.down * moveDistance;
            default: return originalPosition;
        }
    }

    private IEnumerator MasterAnimationRoutine(bool isPlayingIn)
    {
        do
        {
            if (isPlayingIn && startDelay > 0f)
            {
                yield return new WaitForSeconds(startDelay);
            }

            Vector2 targetPos = isPlayingIn ? originalPosition : GetOffsetPosition();
            float targetAlphaVal = isPlayingIn ? targetAlpha : 0f;
            Vector3 targetScaleVal = isPlayingIn ? targetScale : Vector3.zero;

            Vector2 startPos = rectTransform.anchoredPosition;
            float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;
            Vector3 startScale = transform.localScale;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, time / duration);

                if (animationType == AnimationType.MoveToDirection || animationType == AnimationType.MoveAndFade)
                {
                    rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                }
                if (canvasGroup != null && (animationType == AnimationType.FadeCanvas || animationType == AnimationType.MoveAndFade))
                {
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlphaVal, t);
                }
                if (animationType == AnimationType.CustomPopScale)
                {
                    transform.localScale = Vector3.Lerp(startScale, targetScaleVal, t);
                }

                yield return null;
            }

            if (animationType == AnimationType.MoveToDirection || animationType == AnimationType.MoveAndFade)
                rectTransform.anchoredPosition = targetPos;
            if (canvasGroup != null && (animationType == AnimationType.FadeCanvas || animationType == AnimationType.MoveAndFade))
                canvasGroup.alpha = targetAlphaVal;
            if (animationType == AnimationType.CustomPopScale)
                transform.localScale = targetScaleVal;

            if (isLooping)
            {
                isPlayingIn = !isPlayingIn;
            }

        } while (isLooping);

        currentAnim = null;
    }
}