using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores data about a PlayerAction.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/ActionData")]
public class ActionData : ScriptableObject
{
    public ActionManager.EPlayerAction actionType;
    public Sprite uiSprite;
}
