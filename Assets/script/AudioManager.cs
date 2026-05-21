using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // TAMBAHKAN INI UNTUK MENGAKSES SLIDER UI

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background; 
    public AudioClip click;
    public AudioClip hover;
    public AudioClip hit;
    public AudioClip shoot;
    public AudioClip walk;
    public AudioClip closeDoor;
    public AudioClip openDoor;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Jalankan atau matikan BGM berdasarkan scene
        if (scene.name == "Title" || scene.name == "MainMenu")
        {
            PlayBGM(background);
        }
        else
        {
            StopBGM();
        }

        // 2. OTOMATIS CARI SLIDER DI SCENE YANG BARU DI-LOAD
        FindAndSetupSliders();
    }

    /// <summary>
    /// Fungsi otomatis untuk mencari Slider Audio di scene yang aktif berdasarkan Tag atau Nama
    /// </summary>
    private void FindAndSetupSliders()
    {
        // Cari komponen AudioControl yang ada di scene baru (jika Anda meletakkannya di Canvas Menu)
        AudioControl audioControl = FindObjectOfType<AudioControl>();

        if (audioControl != null)
        {
            // Ambil nilai simpanan volume (atau gunakan default 0.75f jika belum ada)
            float savedMaster = PlayerPrefs.GetFloat("masterVolume", 0.75f);
            float savedMusic = PlayerPrefs.GetFloat("musicVolume", 0.75f);
            float savedSfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

            // Hubungkan fungsi Slider ke AudioControl secara dinamis lewat kode (Listeners)
            if (audioControl.masterSlider != null)
            {
                audioControl.masterSlider.value = savedMaster;
                audioControl.masterSlider.onValueChanged.RemoveAllListeners();
                audioControl.masterSlider.onValueChanged.AddListener(audioControl.SetMasterVolume);
            }

            if (audioControl.musicSlider != null)
            {
                audioControl.musicSlider.value = savedMusic;
                audioControl.musicSlider.onValueChanged.RemoveAllListeners();
                audioControl.musicSlider.onValueChanged.AddListener(audioControl.SetMusicVolume);
            }

            if (audioControl.sfxSlider != null)
            {
                audioControl.sfxSlider.value = savedSfx;
                audioControl.sfxSlider.onValueChanged.RemoveAllListeners();
                audioControl.sfxSlider.onValueChanged.AddListener(audioControl.SetSfxVolume);
            }
            
            Debug.Log("AudioManager: Slider Audio berhasil ditemukan dan dihubungkan secara otomatis!");
        }
    }

    // ========================================================
    // SISA KODE FUNGSI BGM DAN SFX ANDA SEBELUMNYA (TETAP SAMA)
    // ========================================================
    
    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true; 
        musicSource.Play();
    }

    public void StopBGM()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusicOnce(AudioClip clip, float volume = 1f)
    {
        if (clip != null) musicSource.PlayOneShot(clip, volume);
    }

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource.clip != clip)
        {
            sfxSource.clip = clip;
            sfxSource.loop = true;
        }
        if (!sfxSource.isPlaying) sfxSource.Play();
    }

    public void StopLoopingSFX()
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
            sfxSource.clip = null;
            sfxSource.loop = false;
        }
    }
}