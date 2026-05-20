using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIBlinkEffect : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("Kecepatan kedipan (semakin kecil nilainya, semakin cepat berkedip)")]
    [SerializeField] private float duration = 1.0f;

    [Range(0f, 1f)] [SerializeField] private float minAlpha = 0.0f;
    [Range(0f, 1f)] [SerializeField] private float maxAlpha = 1.0f;

    private CanvasGroup canvasGroup;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        // Mulai berkedip otomatis saat objek aktif
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
    }

    private IEnumerator BlinkRoutine()
    {
        float time = 0f;
        bool fadingOut = true;

        // Set nilai awal ke maksimal agar tulisan langsung kelihatan saat game mulai
        canvasGroup.alpha = maxAlpha;

        while (true) // Loop abadi selama objek aktif
        {
            float targetAlpha = fadingOut ? minAlpha : maxAlpha;
            float startAlpha = canvasGroup.alpha;
            time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                // Menggunakan SmoothStep agar perpindahan memudar terasa sangat halus/estetik
                canvasGroup.alpha = Mathf.SmoothStep(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            fadingOut = !fadingOut; // Balikkan arah fade (muncul -> hilang -> muncul)
        }
    }
}