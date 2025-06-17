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
        UpdateBuildingVisuals();
    }

    public void ConsumeEnergy(int amount)
    {
        neededEnergy += amount;
        UpdateUI();
        UpdateBuildingVisuals();
    }

    private void UpdateUI()
    {
        currentEnergyText.text = $"Producing: {currentEnergy}";
        neededEnergyText.text = $"Needed: {neededEnergy}";
    }

    public bool IsInDeficit()
    {
        return neededEnergy > currentEnergy;
    }

    public void UpdateBuildingVisuals()
    {
        foreach (GameObject building in Shop.placedBuildings)
        {
            bool isHouse = building.CompareTag("House");
            var spriteRenderers = building.GetComponentsInChildren<SpriteRenderer>();

            if (IsInDeficit() && isHouse)
            {
                // Grey it out
                foreach (var sr in spriteRenderers)
                {
                    sr.color = Color.gray;
                }
            }
            else if (isHouse)
            {
                // Restore normal
                foreach (var sr in spriteRenderers)
                {
                    sr.color = Color.white;
                }
            }
        }
    }
}