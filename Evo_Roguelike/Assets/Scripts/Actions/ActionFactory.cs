using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for creating player actions.
/// </summary>
public class ActionFactory
{
    public static PlayerAction CreatePlayerAction(ActionManager.EPlayerAction actionType)
    {
        PlayerAction playerAction = null;
        switch (actionType)
        {
            case ActionManager.EPlayerAction.Move:
            {
                playerAction = new MoveAction();
                break;
            }
            case ActionManager.EPlayerAction.Feed:
            {
                playerAction = new FeedAction();
                break;
            }
            case ActionManager.EPlayerAction.Attack:
            {
                playerAction = new AttackAction();
                break;
            }
            case ActionManager.EPlayerAction.Null:
            {
                playerAction = new NullAction();
                break;
            }
        }

        return playerAction;
    }
}
