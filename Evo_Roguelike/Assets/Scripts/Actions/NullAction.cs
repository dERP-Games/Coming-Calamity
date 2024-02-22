using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Null object pattern.
/// This class serves as a null object with no implementation.
/// </summary>
public class NullAction : PlayerAction
{
    public NullAction()
    {
        actionType = ActionManager.EPlayerAction.Null;
    }
    public override void OnActionSelect()
    {
        
    }

    public override void OnClick(Vector2 mousePos)
    {
        
    }

    public override void OnHover(Vector2 mousePos)
    {
        
    }

}
