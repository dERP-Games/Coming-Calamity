using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class SpeciesStatTests
{
    // Creates a species stat manager object
    SpeciesStatsManager GenerateStatManager()
    {
        // Create game object
        GameObject go = new GameObject();
        
        // Instantiate stats manager as component
        SpeciesStatsManager statsManager = go.AddComponent<SpeciesStatsManager>();
        statsManager.SpeciesStats = new Dictionary<SpeciesStatsManager.SpeciesStatGroups, Dictionary<string, int>>();

        // Instantiate stats setup
        SpeciesStatsSetup statsSetup = (SpeciesStatsSetup)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/TestStatsSetup.asset", typeof(SpeciesStatsSetup));
        statsManager.statSetup = statsSetup;

        // Return the manager
        return statsManager;
    }

    // Test that stats are properly being initialized to what
    // is configured in the stats setup object
    [Test]
    public void TestStatInitialization()
    {
        // Get stats manager and initialize stats
        SpeciesStatsManager statsManager = GenerateStatManager();
        statsManager.InitSpeciesStats();

        // Iterate through each stat and ensure they are the same
        // as what is specified in the configured object
        foreach(SpeciesStatsConfig config in statsManager.statSetup.statConfigs)
        {
            foreach(SpeciesStat stat in config.statsList)
            {
                Assert.AreEqual(stat.defaultValue, statsManager.GetSpeciesStat(config.statGroup, stat.statName));
            }
        }
    }

    // Test that stats can be properly changed
    [Test]
    public void TestStatModification()
    {
        // Get stats manager and initialize stats
        SpeciesStatsManager statsManager = GenerateStatManager();
        statsManager.InitSpeciesStats();

        // Get initial value from walking stat
        // and expected value that will come from modification
        int initialStatValue = statsManager.GetSpeciesStat(SpeciesStatsManager.SpeciesStatGroups.Mobility, "Walking");
        int expectedNewValue = initialStatValue + 2;

        // Set new stat based on expected value
        // and then get the newly set value
        statsManager.SetSpeciesStat(SpeciesStatsManager.SpeciesStatGroups.Mobility, "Walking", expectedNewValue);
        int newValue = statsManager.GetSpeciesStat(SpeciesStatsManager.SpeciesStatGroups.Mobility, "Walking");

        // Ensure the new value and expected value are the same
        Assert.AreEqual(expectedNewValue, newValue);
    }
}
