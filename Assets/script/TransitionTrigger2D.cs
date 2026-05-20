using UnityEngine;

public class TransitionTrigger2D : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deteksi apakah yang menabrak adalah Player
        if (collision.CompareTag("Player"))
        {
            if (SceneTransitionManager.Instance != null)
            {
                // Memicu custom otomatis pindah scene lewat manager
                SceneTransitionManager.Instance.LoadScene(targetSceneName);
            }
        }
    }
}