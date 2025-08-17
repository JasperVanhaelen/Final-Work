using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FootprintAnchor { BottomLeft, BottomCenter }

public class BuildingFootprint : MonoBehaviour
{
    public int width = 1;
    public int height = 1;
    public FootprintAnchor anchor = FootprintAnchor.BottomCenter; // <- default for iso art
}