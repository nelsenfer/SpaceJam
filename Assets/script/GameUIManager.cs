using System.Collections;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Main Menu Elements")]
    [SerializeField] private AdvancedUIAnimation movingImage;
    [SerializeField] private AdvancedUIAnimation playButton;
    [SerializeField] private AdvancedUIAnimation settingButton;
    [SerializeField] private AdvancedUIAnimation creditButton;
    [SerializeField] private AdvancedUIAnimation exitButton;

    // Panggil fungsi ini jika ingin memunculkan menu lewat kode
    public void OpenMenuUI()
    {
        if (movingImage != null) movingImage.PlayIn();
        if (playButton != null) playButton.PlayIn();
        if (settingButton != null) settingButton.PlayIn();
        if (creditButton != null) creditButton.PlayIn();
        if (exitButton != null) exitButton.PlayIn();
    }

    // Panggil fungsi ini untuk menyembunyikan menu lewat kode
    public void CloseMenuUI()
    {
        if (movingImage != null) movingImage.PlayOut();
        if (playButton != null) playButton.PlayOut();
        if (settingButton != null) settingButton.PlayOut();
        if (creditButton != null) creditButton.PlayOut();
        if (exitButton != null) exitButton.PlayOut();
    }

    // Fungsi pembantu jika tombol Play diklik untuk pindah scene
    public void StartGameTransition(string targetScene)
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("SceneTransitionManager tidak ditemukan di Scene!");
        }
    }
}