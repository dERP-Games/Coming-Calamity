using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// The player-facing container that displays all of the
// available traits the player can choose from. It interfaces
// with the different trait trees to determine which traits
// are now available for picking.
public class TraitPanel : MonoBehaviour
{
    public TraitTree mobilityTree;
    public TraitContainer mobilityContainer;

    public TraitTree durabilityTree;
    public TraitContainer durabilityContainer;

    public TraitTree ferocityTree;
    public TraitContainer ferocityContainer;

    public TextMeshProUGUI descriptionText;

    // This normally starts disabled - it is a sort of pop-up UI element.
    // When it becomes enabled, it refreshes what information needs
    // to be displayed to the player based on the tree status.
    private void OnEnable()
    {
        TraitNode nextNode = mobilityTree.GetNextAvailableTrait();
        mobilityContainer.SetTrait(nextNode.trait);

        nextNode = durabilityTree.GetNextAvailableTrait();
        durabilityContainer.SetTrait(nextNode.trait);

        nextNode = ferocityTree.GetNextAvailableTrait();
        ferocityContainer.SetTrait(nextNode.trait);
    }

    /* Handle to update the description in the panel to match
        a specific trait a player is hovering over.
        Inputs:
            string text - The text for the description that needs to be shown to the player.
     */
    public void UpdateDescription(string text)
    {
        descriptionText.text = text;
    }
}
