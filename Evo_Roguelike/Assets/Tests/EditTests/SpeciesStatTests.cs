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
        statsManager.SpeciesStats = new Dictionary<SpeciesStatsManager.SpeciesStatGroups, StatData>();

        // Instantiate stats setup
        SpeciesStatsSetup statsSetup = (SpeciesStatsSetup)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/TestStatsSetup.asset", typeof(SpeciesStatsSetup));
        statsManager.statSetup = statsSetup;

        // Return the manager
        return statsManager;
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
        MobilityStats initialStatValue = (MobilityStats) statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Mobility);
        MobilityStats modifyingValue = new MobilityStats();
        modifyingValue.walkSpeed = 2.0f;
        var expectedNewValue = initialStatValue + modifyingValue;

        // Set new stat based on expected value
        // and then get the newly set value
        statsManager.SetSpeciesStat(SpeciesStatsManager.SpeciesStatGroups.Mobility, expectedNewValue);
        MobilityStats newValue = (MobilityStats)statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Mobility);

        // Ensure the new value and expected value are the same
        Assert.AreEqual(expectedNewValue, newValue);
    }
}
