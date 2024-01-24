using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: SpeciesStatsManager
 * USAGE: Manager for handling population stats that are
 * modified through adaptations in gameplay.
 */
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
    public SpeciesStatsSetup statSetup;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionary and stat values
        _SpeciesStats = new Dictionary<SpeciesStatGroups, Dictionary<string, int>>();
        InitSpeciesStats();
    }

    /*
	USAGE: Used for getting stat groups
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
	OUTPUT: Dictionary<string, int>, dictionary containing all stat values pertaining to requested group
	*/
    public Dictionary<string, int> GetSpeciesStatGroup(SpeciesStatGroups statGroup)
    {
        return _SpeciesStats[statGroup];
    }

    /*
	USAGE: Used for getting specific stat from group
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
    -   string statName -> name of stat
	OUTPUT: int, current value of requested stat
	*/
    public int GetSpeciesStat(SpeciesStatGroups statGroup, string statName)
    {
        return _SpeciesStats[statGroup][statName];
    }

    /*
	USAGE: Used for setting stat in group
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
    -   string statName -> name of stat
    -   int statValue -> new value being set for the stat
	OUTPUT: ---
	*/
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

    /*
	USAGE: Initialize all stats from configs to their default value
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void InitSpeciesStats()
    {
        // Get all configs from the stat setup object
        List<StatsConfig> configs = statSetup.statConfigs;

        // Iterate through each stat in each group
        // and set their value to the configured default
        foreach(StatsConfig config in configs)
        {
            foreach(SpeciesStat stat in config.statsList)
            {
                SetSpeciesStat(config.statGroup, stat.statName, stat.defaultValue);
            }
        }
    }
}
