using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public int CurrentPopulation { get; private set; } = 0;
    public int CurrentEcoScore { get; private set; } = 0;

    [Header("UI")]
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI ecoScoreText;

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
    }

    public void AddEcoScore(int amount)
    {
        CurrentEcoScore += amount;
        UpdateUI();
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