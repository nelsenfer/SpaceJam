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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
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