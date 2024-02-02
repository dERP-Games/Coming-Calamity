using UnityEngine;

// Scriptable object that defines the data for a trait
// A trait is defined as some configuration of stats and descriptive
// information (i.e. title, image, etc)
[CreateAssetMenu(menuName = "ScriptableObjects/Traits/Trait")]
public class Trait : ScriptableObject
{
    public string traitName;
    public string description;
    public SpeciesStatsManager.SpeciesStatGroups statGroup;
    public SpeciesStatsConfig statsConfig;
    public Sprite icon;
}
