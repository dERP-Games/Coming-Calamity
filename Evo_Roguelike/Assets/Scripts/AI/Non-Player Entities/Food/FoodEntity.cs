using UnityEngine;

/*
 Monobehavior interface for the food entities in the world
 */
public class FoodEntity : MonoBehaviour
{

    [SerializeField]
    public FoodData foodData;

    [SerializeField]
    public EntityViewController viewController;

    [SerializeField]
    private int maxPortions;
    private int portions;
    private int portionRatio;

    private void Awake()
    {
        portionRatio = maxPortions / 4;
    }


    // Test if the species can consume this
    public bool CanBeConsumed(DurabilityStats creatureDurability, FerocityStats creatureFerocity)
    {
        bool canBeTolerated = creatureDurability.digestion > foodData.toxicity;
        bool canBeChewed = creatureFerocity.fang_damage > foodData.ingestionDifficulty;
        bool foodStillAvailable = portions > 0;
        return canBeTolerated && canBeChewed && foodStillAvailable;
    }

    // Test if the species can even access this
    public bool CanBeAccessed(MobilityStats creatureMobility)
    {
        return creatureMobility.reach + creatureMobility.size > foodData.accessDifficulty;
    }

    // Reduce the number of portions that is available from this food source
    public void ConsumePortion()
    {
        portions--;

        // Send a message that we need to update the visible state of this food source
        if (portions % portionRatio == 0 || portions == 0)
        {
            viewController.NextSprite();
        }
    }
}
