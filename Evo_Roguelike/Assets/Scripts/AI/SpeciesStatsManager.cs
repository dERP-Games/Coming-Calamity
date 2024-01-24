using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesStatsManager : MonoBehaviour
{
    // Enum for stat tree types
    public enum SpeciesStatGroups
    {
        Mobility,
        Durability,
        Ferocity
    }

    // Dict to hold all stats for creature population
    Dictionary<SpeciesStatGroups, Dictionary<string, int>> _SpeciesStats;

    // List of stat configurations used for setting stat structure in editor
    public StatsSetup statSetup;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionary and stat values
        _SpeciesStats = new Dictionary<SpeciesStatGroups, Dictionary<string, int>>();
        InitSpeciesStats();
    }

    // Method used for getting stat groups
    public Dictionary<string, int> GetSpeciesStatGroup(SpeciesStatGroups statGroup)
    {
        return _SpeciesStats[statGroup];
    }

    // Method used for getting specific stat from group
    public int GetSpeciesStat(SpeciesStatGroups statGroup, string statName)
    {
        return _SpeciesStats[statGroup][statName];
    }

    // Method used for setting stat in group
    public void SetSpeciesStat(SpeciesStatGroups statGroup, string statName, int statValue)
    {
        // Check if stat group exists in dict
        // and if it doesnt, initialize it
        if(!_SpeciesStats.ContainsKey(statGroup))
        {
            _SpeciesStats.Add(statGroup, new Dictionary<string, int>());
        }

        // Set value of specified stat
        _SpeciesStats[statGroup].Add(statName, statValue);
    }

    // Method used for initializing all species stats
    void InitSpeciesStats()
    {
        List<StatsConfig> configs = statSetup.statConfigs;

        foreach(StatsConfig config in configs)
        {
            foreach(Stat stat in config.statsList)
            {
                SetSpeciesStat(config.statGroup, stat.statName, stat.defaultValue);
            }
        }
    }
}
