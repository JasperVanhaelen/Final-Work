using UnityEngine;
using TMPro;
using System;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public int CurrentPopulation { get; private set; } = 0;
    public int CurrentEcoScore { get; private set; } = 0;

    [Header("UI")]
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI ecoScoreText;

    public static event Action<int> OnEcoScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddPopulation(int amount)
    {
        CurrentPopulation += amount;
        UpdateUI();

        // Update mission progress with the new total population
        MissionManager.Instance.UpdateProgress(MissionType.ReachPopulation, CurrentPopulation);
    }

    public void AddEcoScore(int amount)
    {
        CurrentEcoScore += amount;
        UpdateUI();

        // Update mission progress with the new total eco score
        MissionManager.Instance.UpdateProgress(MissionType.ReachEcoScore, CurrentEcoScore);

        OnEcoScoreChanged?.Invoke(CurrentEcoScore); // Notify listeners
    }

    private void UpdateUI()
    {
        populationText.text = CurrentPopulation.ToString();
        ecoScoreText.text = CurrentEcoScore.ToString();

        if (CurrentEcoScore > 0)
        {
            ecoScoreText.color = new Color(0.0f, 0.4f, 0.0f); // dark green
        }
        else if (CurrentEcoScore < 0)
        {
            ecoScoreText.color = Color.red;
        }
        else
        {
            ecoScoreText.color = Color.white; // Neutral
        }
    }
}