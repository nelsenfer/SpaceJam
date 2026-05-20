using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private GameObject transitionCanvas; 
    private List<AdvancedUIAnimation> elements = new List<AdvancedUIAnimation>(); 

    [Header("Opening Game Settings")]
    [Tooltip("Jika dicentang, transisi akan membuka otomatis saat game pertama kali dinyalakan.")]
    [SerializeField] private bool playOutOnGameStart = true;

    [Header("Timing Settings")]
    [SerializeField] private float waitOnClosed = 0.3f; 

    [Header("Custom Trigger Events")]
    public UnityEvent OnTransitionClosed;
    public UnityEvent OnTransitionOpened;

    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            
            // 1. Cari kontainer transisi bertag 'TransitionCanvas'
            FindTransitionCanvasByTag();

            // 2. Kunci kontainer transisi agar ikut menyeberang antar scene tanpa hancur
            if (transitionCanvas != null) 
            {
                DontDestroyOnLoad(transitionCanvas); 
            }

            // 3. Daftarkan elemen animasi 'atas' dan 'bawah' secara otomatis
            FindAndRegisterTransitionElements();
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (playOutOnGameStart)
        {
            TriggerPlayOut();
        }
        else
        {
            InstantShowTransition();
        }
    }

    // --- FUNGSI UTILITY INTERNAL (PENCARIAN AUTOMATIC) ---

    private void FindTransitionCanvasByTag()
    {
        transitionCanvas = GameObject.FindWithTag("TransitionCanvas");
        
        if (transitionCanvas == null)
        {
            Debug.LogWarning("SceneTransitionManager: GameObject dengan tag 'TransitionCanvas' tidak ditemukan!");
        }
    }

    public void FindAndRegisterTransitionElements()
    {
        // Segarkan ulang canvas jika referensinya sempat hilang akibat perpindahan scene struktural
        if (transitionCanvas == null)
        {
            FindTransitionCanvasByTag();
        }

        if (transitionCanvas != null)
        {
            ClearElements();

            // Mengambil semua komponen AdvancedUIAnimation yang ada di objek anak (atas & bawah)
            AdvancedUIAnimation[] foundElements = transitionCanvas.GetComponentsInChildren<AdvancedUIAnimation>();
            foreach (var element in foundElements)
            {
                RegisterElement(element);
            }
        }
    }

    public void RegisterElement(AdvancedUIAnimation animElement)
    {
        if (!elements.Contains(animElement))
        {
            elements.Add(animElement);
        }
    }

    public void ClearElements()
    {
        elements.Clear();
    }

    // Otomatis dipanggil Unity tepat setelah pergantian scene selesai
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // SOLUSI UTAMA: Segarkan daftar elemen gambar atas & bawah di scene yang baru dimuat
        FindAndRegisterTransitionElements();

        if (isTransitioning)
        {
            StartCoroutine(PlayOutAfterSceneLoad());
        }
    }

    private IEnumerator PlayOutAfterSceneLoad()
    {
        // Jeda waktu aman untuk sinkronisasi frame pertama di scene baru
        yield return new WaitForSeconds(0.15f);

        // Jalankan animasi membuka layar
        foreach (var anim in elements)
        {
            if (anim != null) anim.PlayOut();
        }

        float calculatedWait = (elements.Count > 0 && elements[0] != null) ? elements[0].duration : 0.5f;
        yield return new WaitForSeconds(calculatedWait);

        OnTransitionOpened?.Invoke();
        isTransitioning = false; 
    }

    // --- PEMICU PINDAH SCENE UTAMA ---
    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return; 
        StartCoroutine(TransitionSequence(sceneName));
    }

    private IEnumerator TransitionSequence(string sceneName)
    {
        isTransitioning = true;

        // Pastikan elemen terdata valid sebelum animasi dimulai
        FindAndRegisterTransitionElements();

        // 1. ANIMASI MASUK (Menutup Layar)
        foreach (var anim in elements) 
        {
            if (anim != null) anim.PlayIn();
        }
        
        float calculatedWait = (elements.Count > 0 && elements[0] != null) ? elements[0].duration : 0.5f;
        yield return new WaitForSeconds(calculatedWait + waitOnClosed);
        OnTransitionClosed?.Invoke();

        // 2. PINDAH SCENE
        SceneManager.LoadScene(sceneName);
    }

    // --- CUSTOM TRIGGER VIA KODE ---
    public void InstantHideTransition()
    {
        foreach (var anim in elements)
        {
            if (anim != null) anim.InstantReset();
        }
    }

    public void InstantShowTransition()
    {
        foreach (var anim in elements)
        {
            if (anim != null)
            {
                anim.PlayIn(); 
                anim.StopAllCoroutines(); 
            }
        }
    }

    public void TriggerPlayIn()
    {
        StartCoroutine(PlayInSequence());
    }

    private IEnumerator PlayInSequence()
    {
        foreach (var anim in elements)
        {
            if (anim != null) anim.PlayIn();
        }
        float calculatedWait = (elements.Count > 0 && elements[0] != null) ? elements[0].duration : 0.5f;
        yield return new WaitForSeconds(calculatedWait);
        OnTransitionClosed?.Invoke();
    }

    public void TriggerPlayOut()
    {
        StartCoroutine(PlayOutSequence());
    }

    private IEnumerator PlayOutSequence()
    {
        // Pastikan data elemen divalidasi ulang sebelum pemicuan manual luar siklus load
        FindAndRegisterTransitionElements();

        foreach (var anim in elements)
        {
            if (anim != null) anim.PlayOut();
        }
        float calculatedWait = (elements.Count > 0 && elements[0] != null) ? elements[0].duration : 0.5f;
        yield return new WaitForSeconds(calculatedWait);
        OnTransitionOpened?.Invoke();
    }
}