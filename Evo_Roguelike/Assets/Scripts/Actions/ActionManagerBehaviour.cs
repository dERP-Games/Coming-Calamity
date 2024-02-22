using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monobehavior wrapper for ActionManager.
/// This class is a service.
/// </summary>
public class ActionManagerBehaviour : MonoBehaviour
{
    public List<ActionData> playerActions;

    private ActionManager _actionManager;

    private PlayerAction _currentAction = ActionFactory.CreatePlayerAction(ActionManager.EPlayerAction.Null); // Starts with a null action

    public ActionManager ActionManager
    {
        get
        {
            // Creates hazard manager object on first call
            if (_actionManager == null)
            {
                TimeManager timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
                _actionManager = new ActionManager(playerActions, timeManager);
            }

            return _actionManager;
        }
    }

    private void Start()
    {
        ActionManager.OnStart();
    }

    public void SetCurrentAction(PlayerAction playerAction)
    {
        _currentAction = playerAction;
    }

    public void Update()
    {
        _currentAction.OnHover(Mouse.current.position.ReadValue());

        if(Input.GetMouseButtonDown(0))
        {
            _currentAction.OnClick(Mouse.current.position.ReadValue());
        }
    }
}
