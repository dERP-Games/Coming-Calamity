using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This action stores a target tile info.
/// </summary>
public class MoveAction : PlayerAction
{
    public GroundData targetTile = null;

    private ActionManagerBehaviour _actionManagerBehaviour = null;
    public MoveAction()
    {
        actionType = ActionManager.EPlayerAction.Move;
    }

    public override void OnActionSelect()
    {
        _actionManagerBehaviour = ServiceLocator.Instance.GetService<ActionManagerBehaviour>();
        if( _actionManagerBehaviour != null )
        {
            _actionManagerBehaviour.SetCurrentAction(this);
        }
    }

    /// <summary>
    /// On a mouse click, check if it's on a tile and store that tile.
    /// </summary>
    /// <param name="mousePos">Screen-space mouse pos</param>
    public override void OnClick(Vector2 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        GridManager gridManager = ServiceLocator.Instance.GetService<GridManagerBehaviour>().GridManager;
        if(gridManager != null)
        {
            targetTile = gridManager.GetGroundDataFromWorldPos(worldPos);
        }

        _actionManagerBehaviour.ActionManager.AddAction(this);
        _actionManagerBehaviour.SetCurrentAction(ActionFactory.CreatePlayerAction(ActionManager.EPlayerAction.Null));
    }

    public override void OnHover(Vector2 mousePos)
    {
        
    }
}
