using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// AudioSliderLinker — Otomatis mencari Slider di scene yang baru di-load,
/// lalu menghubungkannya ke AudioControl.
///
/// PENTING: Script ini ada di GameObject DontDestroyOnLoad (sama dengan AudioManager).
/// Slider dicari setiap kali scene berganti via OnSceneLoaded, bukan di Start().
/// </summary>
[RequireComponent(typeof(AudioControl))]
public class AudioSliderLinker : MonoBehaviour
{
    [Header("Nama Container di Hierarchy")]
    [Tooltip("Nama GameObject parent yang berisi MasterSlider")]
    public string masterContainerName = "Master_Container";

    [Tooltip("Nama GameObject parent yang berisi MusicSlider")]
    public string musicContainerName  = "Music_Container";

    [Tooltip("Nama GameObject parent yang berisi SfxSlider")]
    public string sfxContainerName    = "SFX_Container";

    [Header("Nama Slider di Dalam Container")]
    public string masterSliderName = "MasterSlider";
    public string musicSliderName  = "MusicSlider";
    public string sfxSliderName    = "SfxSlider";

    private AudioControl _audioControl;

    private void Awake()
    {
        _audioControl = GetComponent<AudioControl>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Dipanggil otomatis setiap kali scene selesai di-load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tunggu 1 frame agar semua GameObject di scene selesai diinisialisasi
        StartCoroutine(LinkAfterFrame());
    }

    private System.Collections.IEnumerator LinkAfterFrame()
    {
        yield return null; // tunggu 1 frame
        LinkAllSliders();
    }

    // -------------------------------------------------------
    // PUBLIC — bisa juga dipanggil manual saat panel dibuka
    // -------------------------------------------------------
    public void LinkAllSliders()
    {
        _audioControl.masterSlider = FindSliderInContainer(masterContainerName, masterSliderName);
        _audioControl.musicSlider  = FindSliderInContainer(musicContainerName,  musicSliderName);
        _audioControl.sfxSlider    = FindSliderInContainer(sfxContainerName,    sfxSliderName);

        ApplySavedValues();
        RegisterListeners();

        Debug.Log("[AudioSliderLinker] Semua slider berhasil ditemukan dan dihubungkan di scene: " 
                  + SceneManager.GetActiveScene().name);
    }

    // -------------------------------------------------------
    // Cari slider di dalam container, fallback ke seluruh scene
    // -------------------------------------------------------
    private Slider FindSliderInContainer(string containerName, string sliderName)
    {
        // Cari container (termasuk inactive)
        GameObject container = FindInactiveObjectByName(containerName);

        if (container != null)
        {
            foreach (Slider s in container.GetComponentsInChildren<Slider>(true))
            {
                if (s.gameObject.name == sliderName)
                {
                    Debug.Log($"[AudioSliderLinker] '{sliderName}' ditemukan di '{containerName}'.");
                    return s;
                }
            }

            // Fallback: slider pertama di container
            Slider fallback = container.GetComponentInChildren<Slider>(true);
            if (fallback != null)
            {
                Debug.LogWarning($"[AudioSliderLinker] '{sliderName}' tidak ditemukan by name, " +
                                 $"pakai slider pertama di '{containerName}'.");
                return fallback;
            }
        }

        // Fallback: cari di seluruh scene by name
        Debug.LogWarning($"[AudioSliderLinker] Container '{containerName}' tidak ada. " +
                         $"Mencari '{sliderName}' di seluruh scene...");
        return FindSliderByName(sliderName);
    }

    private void ApplySavedValues()
    {
        float master = PlayerPrefs.GetFloat("masterVolume", 0.75f);
        float music  = PlayerPrefs.GetFloat("musicVolume",  0.75f);
        float sfx    = PlayerPrefs.GetFloat("sfxVolume",    0.75f);

        // Set posisi visual slider
        if (_audioControl.masterSlider != null) _audioControl.masterSlider.value = master;
        if (_audioControl.musicSlider  != null) _audioControl.musicSlider.value  = music;
        if (_audioControl.sfxSlider    != null) _audioControl.sfxSlider.value    = sfx;

        // Terapkan ke AudioMixer
        _audioControl.SetMasterVolume(master);
        _audioControl.SetMusicVolume(music);
        _audioControl.SetSfxVolume(sfx);
    }

    private void RegisterListeners()
    {
        if (_audioControl.masterSlider != null)
        {
            _audioControl.masterSlider.onValueChanged.RemoveAllListeners();
            _audioControl.masterSlider.onValueChanged.AddListener(_audioControl.SetMasterVolume);
        }
        if (_audioControl.musicSlider != null)
        {
            _audioControl.musicSlider.onValueChanged.RemoveAllListeners();
            _audioControl.musicSlider.onValueChanged.AddListener(_audioControl.SetMusicVolume);
        }
        if (_audioControl.sfxSlider != null)
        {
            _audioControl.sfxSlider.onValueChanged.RemoveAllListeners();
            _audioControl.sfxSlider.onValueChanged.AddListener(_audioControl.SetSfxVolume);
        }
    }

    // Cari GameObject by name termasuk yang inactive, hanya di scene yang aktif
    private GameObject FindInactiveObjectByName(string objName)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene.isLoaded && go.name == objName)
                return go;
        }
        return null;
    }

    private Slider FindSliderByName(string sliderName)
    {
        foreach (Slider s in Resources.FindObjectsOfTypeAll<Slider>())
        {
            if (s.gameObject.scene.isLoaded && s.gameObject.name == sliderName)
                return s;
        }
        Debug.LogError($"[AudioSliderLinker] Slider '{sliderName}' tidak ditemukan di scene manapun!");
        return null;
    }
}