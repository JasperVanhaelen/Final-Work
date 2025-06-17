using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public Button openShopButton;
    public Button closeShopButton;
    public GameObject lockedTxt;
    public GameObject noCoinsTxt;

    [Header("UI Elements")]
    public GameObject energyUI;

    [Header("Gameplay References")]
    public Grid tilemapGrid;
    public CoinManager coinManager;
    public AudioSource sfxSource;
    public AudioClip placeSound;

    [Header("Available Buildings")]
    public List<BuildingData> buildings;

    private GameObject currentDraggedBuilding;
    private Camera mainCamera;
    private bool isDraggingBuilding = false;
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    private BuildingData currentBuildingData;

    public static List<GameObject> placedBuildings = new List<GameObject>();

    private void Start()
    {
        mainCamera = Camera.main;

        openShopButton.onClick.AddListener(() =>
        {
            shopPanel.SetActive(true);
            energyUI.SetActive(false);
        });

        closeShopButton.onClick.AddListener(() =>
        {
            shopPanel.SetActive(false);
            energyUI.SetActive(true);
        });

    }

    private void Update()
    {
        if (isDraggingBuilding && currentDraggedBuilding != null)
        {
            Vector3 screenPos = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            Vector3Int originCell = tilemapGrid.WorldToCell(worldPos);
            currentDraggedBuilding.transform.position = tilemapGrid.GetCellCenterWorld(originCell);

            List<Vector3Int> cellsToOccupy = GetOccupiedCells(originCell, currentDraggedBuilding);

            bool canPlace = true;
            foreach (var cell in cellsToOccupy)
            {
                if (occupiedCells.Contains(cell))
                {
                    canPlace = false;
                    break;
                }
            }

            SetGhostColor(currentDraggedBuilding, canPlace ? Color.green : Color.red);

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
#if UNITY_EDITOR
                || Input.GetMouseButtonUp(0)
#endif
            )
            {
                if (canPlace)
                {
                    foreach (var cell in cellsToOccupy)
                    {
                        occupiedCells.Add(cell);
                    }
                    FinalizePlacement();
                }
                else
                {
                    CancelPlacement();
                }
            }
        }
    }

    public void OnBeginDragBuilding(BuildingData data)
    {
        currentBuildingData = data;

        // Check if unlocked
        if (StatsManager.Instance.CurrentEcoScore < data.requiredEcoScore)
        {
            ShowLockedMessage();
            return;
        }

        // Check if enough coins
        if (coinManager.CurrentCoins >= data.cost)
        {
            currentDraggedBuilding = Instantiate(data.prefab);
            SetGhostMode(currentDraggedBuilding);
            isDraggingBuilding = true;
            TouchCamera.IsCameraLocked = true;
        }
        else
        {
            ShowNoCoinsMessage(); // <--- Use this instead of Debug.Log
        }
    }

    private void FinalizePlacement()
    {
        MakeSolid(currentDraggedBuilding);
        var costComp = currentDraggedBuilding.GetComponent<BuildingCost>();
        if (costComp != null)
        {
            coinManager.SpendCoins(costComp.cost);
        }

        isDraggingBuilding = false;
        TouchCamera.IsCameraLocked = false;

        // Add to list BEFORE visual updates can happen
        Shop.placedBuildings.Add(currentDraggedBuilding);

        if (currentBuildingData != null)
        {
            // Energy logic
            switch (currentBuildingData.energyType)
            {
                case EnergyType.Producer:
                    EnergyManager.Instance.AddEnergy(currentBuildingData.energyAmount);
                    break;
                case EnergyType.Consumer:
                    EnergyManager.Instance.ConsumeEnergy(currentBuildingData.energyAmount);
                    break;
            }

            if (currentBuildingData.populationAmount > 0)
            {
                StatsManager.Instance.AddPopulation(currentBuildingData.populationAmount);
            }

            if (currentBuildingData.ecoScoreImpact != 0)
            {
                StatsManager.Instance.AddEcoScore(currentBuildingData.ecoScoreImpact);
            }
        }

        if (currentBuildingData.isTree)
        {
            MissionManager.Instance.UpdateProgress(MissionType.PlantTrees,
                MissionManager.Instance.GetCurrentProgress(MissionType.PlantTrees) + 1);
        }

        sfxSource.PlayOneShot(placeSound);

        // Null out after using it
        currentDraggedBuilding = null;
        currentBuildingData = null;
    }

    private void CancelPlacement()
    {
        Destroy(currentDraggedBuilding);
        currentDraggedBuilding = null;
        isDraggingBuilding = false;
        TouchCamera.IsCameraLocked = false;
    }

    private List<Vector3Int> GetOccupiedCells(Vector3Int originCell, GameObject building)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        var footprint = building.GetComponent<BuildingFootprint>();
        if (footprint == null) return cells;

        for (int x = 0; x < footprint.width; x++)
        {
            for (int y = 0; y < footprint.height; y++)
            {
                Vector3Int cell = new Vector3Int(originCell.x + x, originCell.y + y, originCell.z);
                cells.Add(cell);
            }
        }

        return cells;
    }

    private void SetGhostMode(GameObject building)
    {
        originalColors.Clear();
        foreach (var renderer in building.GetComponentsInChildren<SpriteRenderer>())
        {
            originalColors[renderer] = renderer.color;
            Color ghost = renderer.color;
            ghost.a = 0.5f;
            renderer.color = ghost;
        }
    }

    private void SetGhostColor(GameObject building, Color color)
    {
        foreach (var renderer in building.GetComponentsInChildren<SpriteRenderer>())
        {
            color.a = 0.5f;
            renderer.color = color;
        }
    }

    private void MakeSolid(GameObject building)
    {
        foreach (var renderer in building.GetComponentsInChildren<SpriteRenderer>())
        {
            if (originalColors.TryGetValue(renderer, out Color original))
            {
                original.a = 1f;
                renderer.color = original;
            }
            else
            {
                Color fallback = renderer.color;
                fallback.a = 1f;
                renderer.color = fallback;
            }
        }

        originalColors.Clear();
    }

    private void ShowLockedMessage()
    {
        if (lockedTxt == null) return;

        lockedTxt.SetActive(true);
        CancelInvoke(nameof(HideLockedMessage)); // Avoid overlap if clicked multiple times
        Invoke(nameof(HideLockedMessage), 5f);   // Hide after 5 seconds
    }

    private void HideLockedMessage()
    {
        if (lockedTxt != null)
            lockedTxt.SetActive(false);
    }
    
    private void ShowNoCoinsMessage()
    {
        if (noCoinsTxt == null) return;

        noCoinsTxt.SetActive(true);
        CancelInvoke(nameof(HideNoCoinsMessage));
        Invoke(nameof(HideNoCoinsMessage), 3f); // Show for 3 seconds
    }

    private void HideNoCoinsMessage()
    {
        if (noCoinsTxt != null)
            noCoinsTxt.SetActive(false);
    }
}