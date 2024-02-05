using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI container for a trait. This provides the player interactivity
// component to selecting and growing in a specific direction
public class TraitContainer : MonoBehaviour
{
    public TextMeshProUGUI traitTitleContainer;
    public Image traitImage;
    public Trait trait;

    /* Allows setting which trait will be displayed by this container.
       Inputs:
        Trait trait - the trait information that this container will hold
     */
    public void SetTrait(Trait trait)
    {
        this.trait = trait;
        UpdateInformation();
    }

    /* Updates the player-facing components with the information held in the object's trait.
     */
    public void UpdateInformation()
    {
        traitTitleContainer.text = trait.traitName;
        traitImage.sprite = trait.icon;
    }

    public void OnHover()
    {
        // OnHover function to highlight this container when the player mouses over it
        // Still to do
    }

    public void OnMakeChoice()
    {
        // OnMakeChoice function to add the values for this trait to the SpeciesStatManager.
        // Still to do - don't know what object to interface with to do this
        
        // Also, close the panel in general.
        transform.parent.gameObject.SetActive(false);
    }


}
