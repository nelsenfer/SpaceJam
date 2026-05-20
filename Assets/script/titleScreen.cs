using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk New Input System Unity 6
using UnityEngine.InputSystem.Utilities; // Diperlukan untuk fungsi onAnyButtonPress
using System;
using System.Collections; // Diperlukan untuk Coroutine (WaitForSeconds)

public class titleScreen : MonoBehaviour
{
    [Header("Scene Target")]
    [Tooltip("Nama scene Main Menu tujuan setelah pemain menekan tombol")]
    [SerializeField] private string targetSceneName = "test UI";

    [Header("Delay Settings")]
    [Tooltip("Jeda waktu (detik) di awal scene sebelum pemain diperbolehkan menekan tombol")]
    [SerializeField] private float inputDelayDuration = 1.0f;

    private IDisposable anyButtonListener;
    private bool hasTriggered = false;
    private bool canPress = false; // Sakelar pengaman input

    private void Start()
    {
        // Memulai hitung mundur jeda input saat scene pertama kali berjalan
        StartCoroutine(EnableInputAfterDelay());
    }

    private void OnEnable()
    {
        // Mulai mendengarkan input tombol APAPUN (Keyboard, Gamepad, Mouse Klik)
        anyButtonListener = InputSystem.onAnyButtonPress.Call(OnAnyInputDetected);
    }

    private void OnDisable()
    {
        // Bersihkan listener saat scene berganti agar tidak terjadi kebocoran memori
        if (anyButtonListener != null)
        {
            anyButtonListener.Dispose();
        }
    }

    private IEnumerator EnableInputAfterDelay()
    {
        // Menahan status input selama durasi yang ditentukan (1 detik)
        yield return new WaitForSeconds(inputDelayDuration);
        canPress = true;
        Debug.Log("<color=green>[Title Screen]</color> Jeda selesai! Sekarang pemain sudah bisa menekan tombol.");
    }

    private void OnAnyInputDetected(InputControl control)
    {
        // MAKSUD LOGIKA: Jika sakelar canPress masih false, atau sudah pernah memicu transisi, ABAIKAN INPUT
        if (!canPress || hasTriggered) return;

        // VALIDASI: Abaikan jika input berasal dari pergerakan mouse (Mouse Delta / Position)
        if (control.name == "delta" || control.name == "position") return;

        hasTriggered = true;
        
        Debug.Log($"<color=yellow>[Title Screen]</color> Tombol terdeteksi: <b>{control.path}</b>. Memulai transisi scene!");

        // Panggil Manager Transisi untuk menutup layar dan pindah ke Main Menu
        TriggerMenuTransition();
    }

    private void TriggerMenuTransition()
    {
        if (SceneTransitionManager.Instance != null)
        {
            // Ambil data elemen transisi yang ada di scene Title Screen secara paksa sebelum pindah
            SceneTransitionManager.Instance.FindAndRegisterTransitionElements();

            // Memanggil fungsi LoadScene kustom kita yang otomatis menutup layar (PlayIn) lalu pindah scene
            SceneTransitionManager.Instance.LoadScene(targetSceneName);
        }
        else
        {
            // Fallback darurat jika lupa menaruh SceneTransitionManager di hierarchy
            Debug.LogWarning("SceneTransitionManager tidak ditemukan di scene ini! Menjalankan LoadScene instan.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
}