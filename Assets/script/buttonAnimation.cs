using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem; // Wajib untuk New Input System Unity 6

public class buttonAnimation : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI Text;
    
    [Tooltip("Gambar penanda (ikon/pointer) yang muncul HANYA saat tombol di-select dengan Controller")]
    public GameObject tombolA; 

    [Header("Color Palettes")]
    public Color originalColor = Color.white; 
    public Color targetColor;

    private bool isSelected = false;

    private void Start()
    {
        // Konversi kode warna Hex #A73D39
        if (ColorUtility.TryParseHtmlString("#A73D39", out Color hexColor))
        {
            targetColor = hexColor;
        }

        // Set warna awal secara instan
        if (Text != null)
        {
            Text.color = originalColor;
        }

        // Sembunyikan objek penanda di awal
        if (tombolA != null)
        {
            tombolA.SetActive(false);
        }

        // --- LOGIKA DETEKSI CONTROLLER / STIK ---
        CheckControllerAndSelect();
    }

    private void CheckControllerAndSelect()
    {
        // Mengecek apakah ada Gamepad/Stick yang sedang aktif terhubung ke PC
        if (Gamepad.current != null)
        {
            Debug.Log("<color=green>[Input System]</color> Controller terdeteksi! Mengaktifkan auto-select tombol.");
            
            if (EventSystem.current != null)
            {
                // Paksa EventSystem memilih tombol ini jika menggunakan controller
                EventSystem.current.SetSelectedGameObject(this.gameObject);
                OnSelect(); 
            }
        }
        else
        {
            Debug.Log("<color=yellow>[Input System]</color> Tidak ada controller terhubung. Menggunakan mode Mouse & Keyboard (Tanpa Auto-Select).");
            
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            
            // Paksa reset kondisi ke awal
            OnDeselect();
        }
    }

    private void Update()
    {
        // Hanya membaca input tombol A/X jika tombol sedang dalam kondisi AKTIF DI-SELECT
        if (isSelected)
        {
            Gamepad currentGamepad = Gamepad.current;
            if (currentGamepad != null)
            {
                if (currentGamepad.buttonSouth.wasPressedThisFrame)
                {
                    ExecuteButtonAction();
                }
            }
        }
    }

    // --- FUNGSI MODULAR INTERAKSI UI ---

    public void OnHover()
    {
        if (Text != null)
        {
            Text.color = targetColor;
        }
    }

    public void OnExit()
    {
        if (!isSelected && Text != null)
        {
            Text.color = originalColor;
        }
    }

    public void OnSelect()
    {
        isSelected = true;
        OnHover(); 

        // PERBAIKAN UTAMA: Hanya aktifkan tombolA jika controller/stik terdeteksi aktif
        if (tombolA != null)
        {
            if (Gamepad.current != null)
            {
                tombolA.SetActive(true); // Muncul jika pakai stik
            }
            else
            {
                tombolA.SetActive(false); // Tetap mati jika terpilih karena klik mouse
            }
        }
    }

    public void OnDeselect()
    {
        isSelected = false;
        if (Text != null)
        {
            Text.color = originalColor;
        }

        if (tombolA != null)
        {
            tombolA.SetActive(false); 
        }
    }

    private void ExecuteButtonAction()
    {
        Debug.Log($"<color=orange>[Instan Input]</color> Tombol <b>{gameObject.name}</b> Ditekan via Gamepad (A/X)!");

        Button btn = GetComponent<Button>();
        if (btn != null && btn.onClick != null)
        {
            btn.onClick.Invoke();
        }
    }
}