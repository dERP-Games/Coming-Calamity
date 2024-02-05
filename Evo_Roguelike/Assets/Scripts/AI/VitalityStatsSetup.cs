using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Public enum for vitality stat types
public enum VitalityStatType
{
    Health,
    Hunger
}

// Struct used for holding intial stat information
[System.Serializable]
public struct VitalityStat
{
    [Tooltip("Stat type")]
    public VitalityStatType statType;

    [Tooltip("Sets median for this stat")]
    public int medianValue;

    [Tooltip("Sets standard deviation for this stat")]
    public int stdDev;
}

/* CLASS: VitalityStatsSetup
 * USAGE: Configurable object used for setting
 * up default values for vitality ranges
 */
[CreateAssetMenu(menuName = "ScriptableObjects/VitalityStatsSetup")]
public class VitalityStatsSetup : ScriptableObject
{
    public List<VitalityStat> statConfigs;
}
