using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private int coins;

    private const string CoinKey = "PlayerCoins";
    private const int StartingCoins = 1300;

    public int CurrentCoins => coins; // read-only property

    void Start()
    {
        // Load saved coins, or set to starting value if first launch
        if (PlayerPrefs.HasKey(CoinKey))
        {
            coins = PlayerPrefs.GetInt(CoinKey);
        }
        else
        {
            coins = StartingCoins;
            PlayerPrefs.SetInt(CoinKey, coins);
        }

        UpdateCoinDisplay();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        UpdateCoinDisplay();
    }

    public void SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            UpdateCoinDisplay();
        }
        else
        {
            Debug.LogWarning("Not enough coins!");
        }
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinKey, coins);
        PlayerPrefs.Save();
    }

    private void UpdateCoinDisplay()
    {
        coinText.text = coins.ToString();
    }

    public void AddTestCoins()
    {
        AddCoins(500);
    }
}