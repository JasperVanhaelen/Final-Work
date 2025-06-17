using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnergyType
{
    None,
    Producer,
    Consumer
}

[CreateAssetMenu(menuName = "CityBuilder/BuildingData")]
public class BuildingData : ScriptableObject
{
    public bool isTree;

    public string buildingName;
    public GameObject prefab;
    public Sprite icon;
    public int cost;

    // Energy Info
    public EnergyType energyType = EnergyType.None;
    public int energyAmount = 0;

    // pop & eco
    public int populationAmount = 0;
    public int ecoScoreImpact = 0;

    // Unlock requirements
    public int requiredEcoScore = 0; // 0 = always available
}