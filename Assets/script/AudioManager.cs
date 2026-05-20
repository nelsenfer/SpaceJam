using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // untuk menaruh audio(bgm / sfx)
    [Header("---------- Audio Clip ----------")]
    public AudioClip background;
    public AudioClip Click;
    public AudioClip pop;
    public AudioClip paper;
    public static AudioManager Instance { get; private set; }
    




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Agar AudioManager tidak hancur saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // untuk sfx control yang nanti dipanggil
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusicOnce(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            musicSource.PlayOneShot(clip, volume);
        }
    }
}