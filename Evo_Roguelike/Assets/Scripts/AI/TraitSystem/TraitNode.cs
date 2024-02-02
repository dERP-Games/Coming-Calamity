using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Trait object within the trait tree. Expanding this class and diversifying it
// will give us control over how traits are unlocked. This is essentially
// the scaffolding for our tech tree
[CreateAssetMenu(menuName = "ScriptableObjects/Traits/TraitNode")]
public class TraitNode : ScriptableObject
{
    public enum TraitState
    {
        Locked,
        Available,
        Obtained
    }

    public Trait trait;
    public TraitNode child;
    public TraitState state = TraitState.Available;

    [HideInInspector]
    public string guid;
    [HideInInspector]
    public Vector2 position;

    public Trait OnSelection()
    {
        state = TraitState.Obtained;
        return trait;
    }
}
