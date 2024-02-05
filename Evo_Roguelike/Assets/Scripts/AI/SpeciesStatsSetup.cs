using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct used for setting default stat names in editor
[System.Serializable]
public struct SpeciesStatsConfig
{
    [Tooltip("Stats that determine mobility information")]
    public MobilityStats mobilityStats;

    [Tooltip("Stats that identify durability information")]
    public DurabilityStats durabilityStats;

    [Tooltip("Stats that indentify ferocity information")]
    public FerocityStats ferocityStats;
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


// Abstract class for the representing stat data. While this class can't be instantiated,
// it defines how the derivates of this type will interact with each other
public abstract class StatData
{
    protected abstract StatData Add(StatData otherStatData);
    public static StatData operator +(StatData a, StatData b)
    {
        return a.Add(b);
    }
}

// Data class for the Mobility Stats
[System.Serializable]
public class MobilityStats : StatData
{
    public float walkSpeed;
    public float swimSpeed;
    public float flySpeed;

    public float size;
    public float reach;

    protected override StatData Add(StatData other)
    {
        MobilityStats mobStatsOther = other as MobilityStats;
        MobilityStats output = new MobilityStats();
        output.walkSpeed = HelperFunctions.ZeroOrMore(walkSpeed + mobStatsOther.walkSpeed);
        output.swimSpeed = HelperFunctions.ZeroOrMore(swimSpeed + mobStatsOther.swimSpeed);
        output.flySpeed = HelperFunctions.ZeroOrMore(flySpeed + mobStatsOther.flySpeed);
        output.size = HelperFunctions.ZeroOrMore(size + mobStatsOther.size);
        output.reach = HelperFunctions.ZeroOrMore(reach + mobStatsOther.reach);
        return output;
    } 
}


// Data class for the durability stats
[System.Serializable]
public class DurabilityStats : StatData
{
    public float digestion;
    public float resistance;
    public float endurance;

    protected override StatData Add(StatData other)
    {
        DurabilityStats durStatsOther = other as DurabilityStats;
        DurabilityStats output = new DurabilityStats();
        output.digestion = HelperFunctions.ZeroOrMore(digestion + durStatsOther.digestion);
        output.resistance = HelperFunctions.ZeroOrMore(resistance + durStatsOther.resistance);
        output.endurance = HelperFunctions.ZeroOrMore(endurance + durStatsOther.endurance);
        return output;
    }
}


// Data class for the ferocity stats
[System.Serializable]
public class FerocityStats : StatData
{
    public float claw_damage;
    public float intimidation;
    public float fang_damage;
    protected override StatData Add(StatData other)
    {
        FerocityStats durStatsOther = other as FerocityStats;
        FerocityStats output = new FerocityStats();
        output.claw_damage = HelperFunctions.ZeroOrMore(claw_damage + durStatsOther.claw_damage);
        output.intimidation = HelperFunctions.ZeroOrMore(intimidation + durStatsOther.intimidation);
        output.fang_damage = HelperFunctions.ZeroOrMore(fang_damage + durStatsOther.fang_damage);
        return output;
    }
}

/* CLASS: SpeciesStatsSetup
 * USAGE: Configurable object used for setting
 * up default values for species stats
 */
[CreateAssetMenu(menuName = "ScriptableObjects/StatsSetup")]
public class SpeciesStatsSetup : ScriptableObject
{
    public SpeciesStatsConfig statConfigs;
}
