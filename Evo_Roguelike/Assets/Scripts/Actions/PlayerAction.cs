using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for all player actions
/// </summary>
[Serializable]
public abstract class PlayerAction
{
    public ActionManager.EPlayerAction actionType;

    public abstract void OnActionSelect();

    public abstract void OnClick(Vector2 mousePos);
    public abstract void OnHover(Vector2 mousePos);
}
