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
    public List<SpeciesStat> statsList;
}

// Struct used for holding intial stat information
[System.Serializable]
public struct SpeciesStat
{
    [Tooltip("Name of the stat")]
    public string statName;

    [Tooltip("Sets default value for stat")]
    public int defaultValue;
}

/* CLASS: SpeciesStatsSetup
 * USAGE: Configurable object used for setting
 * up default values for species stats
 */
[CreateAssetMenu(menuName = "ScriptableObjects/StatsSetup")]
public class SpeciesStatsSetup : ScriptableObject
{
    public List<StatsConfig> statConfigs;
}
