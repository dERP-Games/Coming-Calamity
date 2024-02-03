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
    Dictionary<SpeciesStatGroups, StatData> _SpeciesStats;

    // Public properties
    public Dictionary<SpeciesStatGroups, StatData> SpeciesStats
    {
        get { return _SpeciesStats; }
        set { _SpeciesStats = value; }
    }

    // List of stat configurations used for setting stat structure in editor
    public SpeciesStatsSetup statSetup;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionary and stat values
        _SpeciesStats = new Dictionary<SpeciesStatGroups, StatData>();
        InitSpeciesStats();
    }

    /*
	USAGE: Used for getting stat groups
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
	OUTPUT: Dictionary<string, int>, dictionary containing all stat values pertaining to requested group
	*/
    public StatData GetSpeciesStatGroup(SpeciesStatGroups statGroup)
    {
        return _SpeciesStats[statGroup];
    }

    /*
	USAGE: Used for setting stat in group
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
    -   string statName -> name of stat
    -   int statValue -> new value being set for the stat
	OUTPUT: ---
	*/
    public void SetSpeciesStat(SpeciesStatGroups statGroup, StatData data)
    {
        if (!_SpeciesStats.ContainsKey(statGroup))
        {
            _SpeciesStats.Add(statGroup, data);
        }
        // Set value of specified stat
        _SpeciesStats[statGroup] = data;
    }

    /*
	USAGE: Used for inheriting new traits - the trait will provide the corresponding
    stat group and this function will make sure the two mesh together
	ARGUMENTS: 
    -	SpeciesStatGroups statGroup -> key to corresponding group of stats in dictionary
    -   StatGroup data -> the data to add
	OUTPUT: ---
	*/
    public void AddToSpeciesStat(SpeciesStatGroups statGroup, StatData data)
    {
        _SpeciesStats[statGroup] += data;
    }

    public void AddTrait(Trait trait)
    {
        SpeciesStatsConfig statData = trait.statsConfig;
        AddToSpeciesStat(SpeciesStatGroups.Mobility, statData.mobilityStats);
        AddToSpeciesStat(SpeciesStatGroups.Durability, statData.durabilityStats);
        AddToSpeciesStat(SpeciesStatGroups.Ferocity, statData.ferocityStats);
    }

    /*
	USAGE: Initialize all stats from configs to their default value
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public void InitSpeciesStats()
    {
        // Set up the stats
        SetSpeciesStat(SpeciesStatGroups.Mobility, statSetup.statConfigs.mobilityStats);
        SetSpeciesStat(SpeciesStatGroups.Durability, statSetup.statConfigs.durabilityStats);
        SetSpeciesStat(SpeciesStatGroups.Ferocity, statSetup.statConfigs.ferocityStats);
    }
}
