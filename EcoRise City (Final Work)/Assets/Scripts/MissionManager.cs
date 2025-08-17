using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject missionsPanel;
    public GameObject missionSlotPrefab;
    public Transform missionSlotParent;

    public GameObject MissionsPanel;
    public Button openMissionsButton;
    public Button closeMissionsButton;

    [Header("Mission Complete Popup")]
    public GameObject completePopup;
    public TextMeshProUGUI completeTxt;
    public Button popupCloseBtn;

    [Header("Game References")]
    public CoinManager coinManager; // Add reference to coin-handling script

    private List<Mission> allMissions = new List<Mission>();
    private List<GameObject> activeSlots = new List<GameObject>();
    private int currentSet = 0;

    private Dictionary<MissionType, int> progressData = new Dictionary<MissionType, int>() {
        { MissionType.PlantTrees, 0 },
        { MissionType.ReachPopulation, 0 },
        { MissionType.ReachEcoScore, 0 }
    };

    public static MissionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeMissions();
        ShowCurrentMissions();

        openMissionsButton.onClick.AddListener(OpenPanel);
        closeMissionsButton.onClick.AddListener(ClosePanel);
        popupCloseBtn.onClick.AddListener(ClosePopup);

    }

    void InitializeMissions()
    {
        allMissions = new List<Mission>() {
            new Mission { description = "Plant 5 trees", type = MissionType.PlantTrees, targetAmount = 5, reward = 300 },
            new Mission { description = "Reach population of 10", type = MissionType.ReachPopulation, targetAmount = 10, reward = 400 },
            new Mission { description = "Obtain eco-score of 15", type = MissionType.ReachEcoScore, targetAmount = 15, reward = 400 },
            new Mission { description = "Plant 10 trees", type = MissionType.PlantTrees, targetAmount = 10, reward = 400 },
            new Mission { description = "Reach population of 25", type = MissionType.ReachPopulation, targetAmount = 25, reward = 800 },
            new Mission { description = "Obtain eco-score of 30", type = MissionType.ReachEcoScore, targetAmount = 30, reward = 800 },
            new Mission { description = "Plant 20 trees", type = MissionType.PlantTrees, targetAmount = 20, reward = 1000 },
            new Mission { description = "Reach population of 50", type = MissionType.ReachPopulation, targetAmount = 50, reward = 1000 }
        };
    }

    void ShowCurrentMissions()
    {
        ClearUI();

        int start = currentSet * 2;

        for (int i = start; i < start + 2 && i < allMissions.Count; i++)
        {
            allMissions[i].isActive = true;

            GameObject slot = Instantiate(missionSlotPrefab, missionSlotParent);
            activeSlots.Add(slot);

            UpdateSlotUI(slot, allMissions[i]);
        }
    }

    void UpdateSlotUI(GameObject slot, Mission mission)
    {
        var desc = slot.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        var slider = slot.transform.Find("ProgressBar").GetComponent<Slider>();
        var reward = slot.transform.Find("RewardText").GetComponent<TextMeshProUGUI>();
        var checkmark = slot.transform.Find("CompletedCheckmark").gameObject;

        int current = progressData[mission.type];

        desc.text = mission.description;
        reward.text = $"Reward: {mission.reward}";
        slider.maxValue = mission.targetAmount;
        slider.value = current;
        checkmark.SetActive(mission.isCompleted);
    }

    void ClearUI()
    {
        foreach (var slot in activeSlots)
            Destroy(slot);

        activeSlots.Clear();
    }

    public void UpdateProgress(MissionType type, int newAmount)
    {
        progressData[type] = newAmount;

        List<Mission> justCompleted = new List<Mission>();

        foreach (var mission in allMissions)
        {
            if (mission.isActive && !mission.isCompleted && mission.type == type)
            {
                if (newAmount >= mission.targetAmount)
                {
                    mission.isCompleted = true;
                    coinManager.AddCoins(mission.reward);
                    justCompleted.Add(mission);
                }
            }
        }

        // Show popups after all completions are processed
        foreach (var mission in justCompleted)
        {
            ShowPopup($"Mission Complete:\n{mission.description}\n+{mission.reward} Coins");
        }

        CheckSetCompletion(); // Now okay to advance to next set
        RefreshUI(); // Refresh after completion and popup
    }

    void RefreshUI()
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            UpdateSlotUI(activeSlots[i], allMissions[currentSet * 2 + i]);
        }
    }

    void CheckSetCompletion()
    {
        int completed = 0;
        int start = currentSet * 2;
        for (int i = start; i < start + 2 && i < allMissions.Count; i++)
        {
            if (allMissions[i].isCompleted) completed++;
        }

        if (completed == 2)
        {
            currentSet++;
            ShowCurrentMissions();
        }
    }

    public void ToggleMissionPanel()
    {
        missionsPanel.SetActive(!missionsPanel.activeSelf);
    }

    void OpenPanel()
    {
        missionsPanel.SetActive(true);
    }

    void ClosePanel()
    {
        missionsPanel.SetActive(false);
    }

    void ShowPopup(string message)
    {
        completeTxt.text = message;
        completePopup.SetActive(true);
    }

    void ClosePopup()
    {
        completePopup.SetActive(false);
    }
    
    public int GetCurrentProgress(MissionType type)
    {
        return progressData.ContainsKey(type) ? progressData[type] : 0;
    }
}