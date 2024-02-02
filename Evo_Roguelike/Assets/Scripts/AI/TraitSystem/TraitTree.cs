using Codice.Client.Common.TreeGrouper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that creates a TraitTree. TODO is to make it editable in editor.
[CreateAssetMenu(menuName = "ScriptableObjects/Traits/TraitTree")]
public class TraitTree : ScriptableObject
{
    public TraitNode rootTrait;
    
    // Tree traversal. This tree doesn't need to be calculating things per frame,
    // only on command. Therefore, this will let us know what is next
    // available for the player in this tree.
    public TraitNode GetNextAvailableTrait()
    {
        TraitNode current = rootTrait;
        bool isChildAvailable = current.child != null && current.child.state != TraitNode.TraitState.Locked;

        // Note: This check currently means that at the end of a tree (i.e. the last leaf), 
        // you will keep getting the same option. Will address this when more
        // content is available
        while (current.state == TraitNode.TraitState.Obtained && isChildAvailable)
        {
            current = current.child;
            isChildAvailable = current.child != null && current.child.state != TraitNode.TraitState.Locked;
        }
        return current;
    }
}
