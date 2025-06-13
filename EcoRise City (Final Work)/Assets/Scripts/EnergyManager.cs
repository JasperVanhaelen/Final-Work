using TMPro;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;

    public int currentEnergy = 0;
    public int neededEnergy = 0;

    [Header("UI References")]
    public TextMeshProUGUI currentEnergyText;
    public TextMeshProUGUI neededEnergyText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        UpdateUI();
    }

    public void ConsumeEnergy(int amount)
    {
        neededEnergy += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentEnergyText.text = $"Producing: {currentEnergy}";
        neededEnergyText.text = $"Needed: {neededEnergy}";
    }
}