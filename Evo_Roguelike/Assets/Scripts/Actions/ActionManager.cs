using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class for the action system
/// </summary>
public class ActionManager
{
    // Possible player actions
    public enum EPlayerAction
    {
        Move,
        Feed,
        Attack,
        Null
    }

    // This delegate is fired when queuedActions gets updated
    public delegate void DQueuedActionsUpdated();
    public DQueuedActionsUpdated dQueuedActionsUpdated;

    // List of all available actions which gets populated into the action bar
    public List<ActionData> availableActions;

    [HideInInspector]
    public List<PlayerAction> queuedActions = new List<PlayerAction>();


    private TimeManager _timeManager;
    private int maxNumberOfQueuedActions = 3; // hard coded limit on actions for now

    public ActionManager(List<ActionData> actions, TimeManager timeManager)
    {
        availableActions = actions;
        _timeManager = timeManager;
    }

    public void OnStart()
    {
        _timeManager.D_tick += OnTick;
    }

    /// <summary>
    /// On new timestep, send queued actions to population and clear it.
    /// </summary>
    private void OnTick()
    {
        // Send actions to population here.
        ServiceLocator.Instance.GetService<PopulationManager>().SetNextActions(queuedActions);

        queuedActions.Clear();
        dQueuedActionsUpdated?.Invoke();
    }

    /// <summary>
    /// Adds action to queuedActions if is is not full.
    /// </summary>
    /// <param name="playerAction">Action to be enqueued</param>
    public void AddAction(PlayerAction playerAction)
    {
        if(queuedActions.Count < maxNumberOfQueuedActions)
        {
            queuedActions.Add(playerAction);
        }

        dQueuedActionsUpdated?.Invoke();
    }

    /// <summary>
    /// Returns corresponding action data for a player action.
    /// </summary>
    /// <param name="playerAction">Action to be queried</param>
    /// <returns>Corresponding ActionData</returns>
    public ActionData GetActionDataFromPlayerAction(PlayerAction playerAction)
    {
        foreach(ActionData actionData in availableActions)
        {
            if(actionData.actionType == playerAction.actionType)
            {
                return actionData;
            }
        }

        return null;
    }

}
