using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUnlockUI : MonoBehaviour
{
    public BuildingData buildingData;
    public GameObject lockOverlay; // Optional lock icon overlay
    public Button buildButton;

    private void Start()
    {
        RefreshUnlockState();
        StatsManager.OnEcoScoreChanged += OnEcoScoreChanged;
    }

    private void OnDestroy()
    {
        StatsManager.OnEcoScoreChanged -= OnEcoScoreChanged;
    }

    private void OnEcoScoreChanged(int newEcoScore)
    {
        RefreshUnlockState();
    }

    private void RefreshUnlockState()
    {
        bool unlocked = StatsManager.Instance.CurrentEcoScore >= buildingData.requiredEcoScore;
        buildButton.interactable = unlocked;

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);

        var icon = GetComponent<Image>();
        if (icon != null)
            icon.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f); // Greyed out
    }
}