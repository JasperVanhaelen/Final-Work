using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BuildingData buildingData;

    private bool dragStarted = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (buildingData == null) return;

        // Tell the shop to start dragging
        FindObjectOfType<Shop>().OnBeginDragBuilding(buildingData);
        dragStarted = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Nothing needed here – the Shop handles the dragging visuals
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragStarted = false;
    }
}