using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedAction : PlayerAction
{
    public FeedAction()
    {
        actionType = ActionManager.EPlayerAction.Feed;
    }

    public override void OnActionSelect()
    {
        throw new System.NotImplementedException();
    }

    public override void OnClick(Vector2 mousePos)
    {
        throw new System.NotImplementedException();
    }

    public override void OnHover(Vector2 mousePos)
    {
        throw new System.NotImplementedException();
    }
}
