using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem; // Tetap wajib ada



public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int totalEnemies;

    [Header("Level Transition")]
    public float transitionDelay = 2f;

    [Header("Pause Settings")]
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    public bool isShooting = true;

    [Header("Game Over Settings")]
    public GameObject loseMenuUI;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Time.timeScale = 1f;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isShooting = false;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isShooting = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // --- KODE LAMA ANDA ---
    public void EnemyDied()
    {
        totalEnemies--;
        if (totalEnemies <= 0) StartCoroutine(LevelCompleteRoutine());
    }

    IEnumerator LevelCompleteRoutine()
    {
        Debug.Log("🎉 LEVEL CLEAR!");
        yield return new WaitForSeconds(transitionDelay);

        // 1. Hitung indeks scene berikutnya
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 2. Cek apakah indeks berikutnya masih ada di dalam Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Ambil jalur (path) scene berdasarkan indeks, lalu potong hanya mengambil namanya saja
            string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            // 3. Panggil transisi lewat Button_Manager
            if (Button_Manager.Instance != null)
            {
                Button_Manager.Instance.goOtherScene(nextSceneName);
            }
            else
            {
                // Fallback jika Button_Manager tidak sengaja terhapus/tidak ada
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
        else
        {
            Debug.LogWarning("Tidak ada scene berikutnya di Build Settings! Kembali ke MainMenu.");
            if (Button_Manager.Instance != null) Button_Manager.Instance.goMainMenu();
        }
    }

    public void GameOver()
    {
        Debug.Log("💀 GAME OVER!");

        if (loseMenuUI != null)
        {
            loseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isShooting = false;
    }
}