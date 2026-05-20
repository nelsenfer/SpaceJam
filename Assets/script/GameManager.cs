using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int totalEnemies;

    [Header("Level Transition")]
    public float transitionDelay = 2f;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void EnemyDied()
    {
        totalEnemies--;

        if (totalEnemies <= 0)
        {
            StartCoroutine(LevelCompleteRoutine());
        }
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
    }
}