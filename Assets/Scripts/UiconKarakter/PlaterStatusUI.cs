using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Identitas Player")]
    public string namaPlayer = "Jokowi";
    public string jabatanSaatIni = "Walikota";
    public string jabatanBerikutnya = "Gubernur";

    [Header("Data Statistik")]
    public int currentPopulation = 0; // DATABARU: Menyimpan jumlah warga
    public float currentXP = 0;
    public float maxXP = 100;
    public int currentMoney = 1500;

    [Header("Referensi UI (Drag dari Hierarchy)")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI targetRankText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI populationText; // SLOT BARU: Untuk Teks Populasi
    public Slider xpSlider;
    public Image playerIcon;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // CHEAT: Tekan SPASI untuk simulasi
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddXP(25);
            AddMoney(500);
            AddPopulation(10); // Nambah 10 warga tiap tekan spasi
        }
    }

    // --- FUNGSI BARU UNTUK NAMBAH WARGA ---
    public void AddPopulation(int amount)
    {
        currentPopulation += amount;
        UpdateUI(); // Update layar agar angka berubah
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        if (currentXP >= maxXP)
        {
            currentXP = 0;
            maxXP *= 1.5f;
            NaikJabatan();
        }

        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    void NaikJabatan()
    {
        if (jabatanSaatIni == "Walikota")
        {
            jabatanSaatIni = "Gubernur";
            jabatanBerikutnya = "Presiden";
        }
        else if (jabatanSaatIni == "Gubernur")
        {
            jabatanSaatIni = "Presiden";
            jabatanBerikutnya = "Penguasa Dunia";
        }
    }

    void UpdateUI()
    {
        if (nameText != null) nameText.text = jabatanSaatIni + " - " + namaPlayer;
        if (targetRankText != null) targetRankText.text = "Target: " + jabatanBerikutnya;
        if (xpSlider != null) xpSlider.value = currentXP / maxXP;
        if (moneyText != null) moneyText.text = "$ " + currentMoney.ToString("N0");

        // --- UPDATE TEKS POPULASI ---
        if (populationText != null)
        {
            // Format N0 memberi titik pemisah ribuan (contoh: 1.000)
            populationText.text = "Populasi: " + currentPopulation.ToString("N0") + " Jiwa";
        }
    }
}