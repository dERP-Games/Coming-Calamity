using System;
using System.Collections.Generic;
using UnityEngine;


/*
 Class that governs the view controller for all animated
 entities in game. This can be used for player creatures (and will likely need expanding)
 as well as food and whatever else we need.
 */
[Serializable]
public class EntityViewController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    List<Sprite> sprites;
    private int currentSprite = 0;

    // Move onto the next sprite on the list.
    public void NextSprite()
    {
        currentSprite++;
        if (currentSprite >= sprites.Count)
        {
            return;
        }

        spriteRenderer.sprite = sprites[currentSprite];
    }
}