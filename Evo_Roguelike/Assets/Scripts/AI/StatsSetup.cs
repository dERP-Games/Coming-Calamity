using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct used for setting default stat names in editor
[System.Serializable]
public struct StatsConfig
{
    [Tooltip("Stat group the list of stats belongs to")]
    public SpeciesStatsManager.SpeciesStatGroups statGroup;

    [Tooltip("List of all stat names in config")]
    public List<Stat> statsList;
}

// Struct used for holding intial stat information
[System.Serializable]
public struct Stat
{
    [Tooltip("Name of the stat")]
    public string statName;

    [Tooltip("Sets default value for stat")]
    public int defaultValue;
}

[CreateAssetMenu(menuName = "ScriptableObjects/StatsSetup")]
public class StatsSetup : ScriptableObject
{
    public List<StatsConfig> statConfigs;
}
