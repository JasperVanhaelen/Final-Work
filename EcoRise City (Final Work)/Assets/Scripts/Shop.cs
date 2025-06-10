using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic; // <-- Needed for HashSet<T>
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button openShopButton;
    public Button closeShopButton;

    public GameObject building1Prefab;
    public GameObject building2Prefab;

    private GameObject currentDraggedBuilding;
    private Camera mainCamera;

    public Grid tilemapGrid;

    private bool isDraggingBuilding = false;

    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    public CoinManager coinManager; // drag your CoinManager GameObject in the Inspector

    private void Start()
    {
        mainCamera = Camera.main;
        openShopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.AddListener(CloseShop);
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

            // Get all the tiles the building would occupy
            List<Vector3Int> cellsToOccupy = GetOccupiedCells(originCell, currentDraggedBuilding);

            // Check if any of them are already occupied
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

            // On release
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
                    FinalizePlacement(); // Lock it in
                }
                else
                {
                    // Cancel placement
                    Destroy(currentDraggedBuilding);
                    currentDraggedBuilding = null;
                    isDraggingBuilding = false;
                    TouchCamera.IsCameraLocked = false;
                }
            }
        }
    }

    public void OnBeginDragBuilding1()
    {
        if (building1Prefab != null)
        {
            int cost = building1Prefab.GetComponent<BuildingCost>().cost;

            if (coinManager.CurrentCoins >= cost)
            {
                currentDraggedBuilding = Instantiate(building1Prefab);
                SetGhostMode(currentDraggedBuilding);
                isDraggingBuilding = true;
                TouchCamera.IsCameraLocked = true;
            }
            else
            {
                Debug.Log("Not enough coins for Building 1.");
            }
        }
    }

    public void OnBeginDragBuilding2()
    {
        if (building2Prefab != null)
        {
            int cost = building2Prefab.GetComponent<BuildingCost>().cost;

            if (coinManager.CurrentCoins >= cost)
            {
                currentDraggedBuilding = Instantiate(building2Prefab);
                SetGhostMode(currentDraggedBuilding);
                isDraggingBuilding = true;
                TouchCamera.IsCameraLocked = true;
            }
            else
            {
                Debug.Log("Not enough coins for Building 1.");
            }
        }
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

    private void FinalizePlacement()
    {
        if (currentDraggedBuilding != null)
        {
            MakeSolid(currentDraggedBuilding);
        }

        if (currentDraggedBuilding != null)
    {
        var costComp = currentDraggedBuilding.GetComponent<BuildingCost>();
        if (costComp != null)
        {
            coinManager.SpendCoins(costComp.cost);
        }
    }
    
        isDraggingBuilding = false;
        currentDraggedBuilding = null;
        TouchCamera.IsCameraLocked = false;
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);
    }

    private void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    private void SetGhostMode(GameObject building)
    {
        originalColors.Clear(); // Clear from previous building

        var renderers = building.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            originalColors[renderer] = renderer.color;

            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }
    }

    private void MakeSolid(GameObject building)
    {
        var renderers = building.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            if (originalColors.ContainsKey(renderer))
            {
                Color color = originalColors[renderer];
                color.a = 1f;
                renderer.color = color;
            }
            else
            {
                // fallback in case we missed one
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }
        }
    
        originalColors.Clear(); // Clear after applying
    }

    private void SetGhostColor(GameObject building, Color color)
    {
        var renderers = building.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            color.a = 0.5f; // still semi-transparent
            renderer.color = color;
        }
    }
}