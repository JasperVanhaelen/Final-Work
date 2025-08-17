using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementHighlighter : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile; // simple transparent tile

    public Color validColor  = new Color(0f, 1f, 0f, 0.2f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.2f);

    public void Show(IList<Vector3Int> cells, bool canPlace)
    {
        tilemap.ClearAllTiles();
        tilemap.color = canPlace ? validColor : invalidColor;
        foreach (var c in cells) tilemap.SetTile(c, tile);
    }

    public void Hide()
    {
        tilemap.ClearAllTiles();
    }
}