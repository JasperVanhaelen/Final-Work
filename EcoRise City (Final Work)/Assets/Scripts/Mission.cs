using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionType {
    PlantTrees,
    ReachPopulation,
    ReachEcoScore
}

[System.Serializable]
public class Mission {
    public string description;
    public MissionType type;
    public int targetAmount;
    public int reward;
    public bool isCompleted;
    public bool isActive;
}