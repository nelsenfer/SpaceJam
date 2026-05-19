using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int totalEnemies;

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
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        Debug.Log("🎉 SEMUA MUSUH TEWAS! LEVEL CLEAR!");
    }

    public void GameOver()
    {
        Debug.Log("💀 PLAYER TEWAS! GAME OVER!");
    }
}