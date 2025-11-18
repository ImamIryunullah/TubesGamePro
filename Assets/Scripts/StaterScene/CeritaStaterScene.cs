using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Wajib untuk pindah scene

public class StoryManager : MonoBehaviour
{
    [Header("Setting Cerita")]
    [TextArea(3, 10)] // Supaya kotak ketiknya luas di Inspector
    public string[] daftarKalimat; // Tempat kita nulis ceritanya nanti

    [Header("Referensi UI")]
    public TextMeshProUGUI textDisplay; // Tarik objek StoryText ke sini

    private int indexKalimat = 0; // Untuk melacak kita ada di kalimat ke berapa

    void Start()
    {
        // Saat mulai, tampilkan kalimat pertama
        TampilkanKalimat();
    }

    // Fungsi ini dipanggil saat tombol Layar Penuh diklik
    public void KlikLanjut()
    {
        indexKalimat++; // Pindah ke kalimat selanjutnya

        // Cek apakah kalimat sudah habis?
        if (indexKalimat < daftarKalimat.Length)
        {
            TampilkanKalimat();
        }
        else
        {
            // KALAU HABIS -> MASUK GAMEPLAY
            MulaiGame();
        }
    }

    void TampilkanKalimat()
    {
        textDisplay.text = daftarKalimat[indexKalimat];
    }

    void MulaiGame()
    {
        Debug.Log("Cerita Selesai, Masuk ke Kota...");
        // GANTI ANGKA 1 DENGAN URUTAN SCENE KOTA KAMU DI BUILD SETTINGS
        SceneManager.LoadScene(1);
    }
}