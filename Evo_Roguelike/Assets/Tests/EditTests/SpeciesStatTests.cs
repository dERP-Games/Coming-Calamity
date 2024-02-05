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

    Trait LoadTestTrait(string traitName)
    {
        Trait trait = (Trait)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Traits/"+traitName+".asset", typeof(Trait));
        return trait;
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

    // Test that traits can be added to stats
    [Test]
    public void TestAddTraits()
    {
        // Arrange
        SpeciesStatsManager statsManager = GenerateStatManager();
        statsManager.InitSpeciesStats();
        MobilityStats initialMobilityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Mobility) as MobilityStats;
        DurabilityStats initialDurabilityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Durability) as DurabilityStats;
        FerocityStats initialFerocityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Ferocity) as FerocityStats;
        float expectedWalkingOutcome = initialMobilityData.walkSpeed + 1.0f;
        float expectedDigestionOutcome = initialDurabilityData.digestion + 1.0f;
        float expectedClawOutcome = initialFerocityData.claw_damage + 1.0f;
        Trait mobilityTrait = LoadTestTrait("TestTrait1");
        Trait durabilityTrait = LoadTestTrait("TestTrait2");
        Trait ferocityTrait = LoadTestTrait("TestTrait3");

        // Test that loaded data is ok
        Assert.AreEqual(mobilityTrait.statGroup, SpeciesStatsManager.SpeciesStatGroups.Mobility);
        Assert.AreEqual(mobilityTrait.statsConfig.mobilityStats.walkSpeed, 1);
        Assert.AreEqual(durabilityTrait.statGroup, SpeciesStatsManager.SpeciesStatGroups.Durability);
        Assert.AreEqual(durabilityTrait.statsConfig.durabilityStats.digestion, 1);
        Assert.AreEqual(ferocityTrait.statGroup, SpeciesStatsManager.SpeciesStatGroups.Ferocity);
        Assert.AreEqual(ferocityTrait.statsConfig.ferocityStats.claw_damage, 1);


        // Act
        statsManager.AddTrait(mobilityTrait);
        statsManager.AddTrait(durabilityTrait);
        statsManager.AddTrait(ferocityTrait);
        MobilityStats newMobilityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Mobility) as MobilityStats;
        DurabilityStats newDurabilityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Durability) as DurabilityStats;
        FerocityStats newFerocityData = statsManager.GetSpeciesStatGroup(SpeciesStatsManager.SpeciesStatGroups.Ferocity) as FerocityStats;

        // Assert
        // Test that what should've changed has changed
        Assert.AreEqual(newMobilityData.walkSpeed, expectedWalkingOutcome);
        Assert.AreEqual(newDurabilityData.digestion, expectedDigestionOutcome);
        Assert.AreEqual(newFerocityData.claw_damage, expectedClawOutcome);

        // Test that other things are unaffected
        Assert.AreEqual(initialDurabilityData.endurance, newDurabilityData.endurance);
        Assert.AreEqual(initialDurabilityData.resistance, newDurabilityData.resistance);
        Assert.AreEqual(initialFerocityData.fang_damage, newFerocityData.fang_damage);
        Assert.AreEqual(initialFerocityData.intimidation, newFerocityData.intimidation);
    }
}
