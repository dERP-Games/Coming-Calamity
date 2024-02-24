using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEditor;

public class ActionSystemETests
{

    private ActionManager _actionManager;
    private TimeManager _timeManager;

    private void InitializeActionManager()
    {
        _timeManager = new TimeManager(1f, true);
        ActionData moveActionData = (ActionData)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Actions/MoveActionData.asset", typeof(ActionData));
        ActionData feedActionData = (ActionData)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Actions/FeedAction.asset", typeof(ActionData));
        ActionData attackActionData = (ActionData)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Actions/AttackAction.asset", typeof(ActionData));

        List<ActionData> actionDatas = new List<ActionData>() {moveActionData, feedActionData, attackActionData };
        _actionManager = new ActionManager(actionDatas, _timeManager);

    }

    /// <summary>
    /// Testing action queue with time manager.
    /// </summary>
    [Test]
    public void TestActionSystem()
    {
        InitializeActionManager();
        _actionManager.OnStart();
        PlayerAction action1 = new MoveAction();
        PlayerAction action2 = new FeedAction();
        PlayerAction action3 = new AttackAction();

        int expectedVal1 = 3;
        _actionManager.AddAction(action1);
        _actionManager.AddAction(action2);
        _actionManager.AddAction(action3);

        Assert.AreEqual(expectedVal1, _actionManager.queuedActions.Count);

        PlayerAction action4 = new MoveAction();
        _actionManager.AddAction(action4);
        Assert.AreEqual(expectedVal1, _actionManager.queuedActions.Count);

        _timeManager.AdvanceTimer();
        int expectedVal2 = 0;
        Assert.AreEqual(expectedVal2, _actionManager.queuedActions.Count);

    }
}
