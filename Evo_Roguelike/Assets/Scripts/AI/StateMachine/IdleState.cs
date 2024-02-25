using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AbsBaseState<CreatureStateMachine.CreatureStates>
{
    // Get movement controls
    KinematicMovement _MovementControls;

    // Constructor with call to base state class
    public IdleState() : base(CreatureStateMachine.CreatureStates.Idle)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        CreatureStateMachine FSM = (CreatureStateMachine)OwnerFSM;
        _MovementControls = FSM.MovementControls;
    }

    /*
	USAGE: Handler for state exit
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void ExitState()
    {
        // Any clean up needed from this state will go here
    }

    /*
	USAGE: Handler for state update ticks
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void UpdateState()
    {
        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        // Enable movement
        _MovementControls.CanMove = false;
        _MovementControls.IsWandering = false;
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: CreatureStates, state to transition to given current game conditions
	*/
    public override CreatureStateMachine.CreatureStates GetNextState()
    {
        // Get the next action from population manager
        PlayerAction curAction = ServiceLocator.Instance.GetService<PopulationManager>().GetCurrentAction();

        // Switch on action types to switch states
        switch (curAction.actionType)
        {
            case ActionManager.EPlayerAction.Move:
                return CreatureStateMachine.CreatureStates.MovingTo;
            case ActionManager.EPlayerAction.Feed:
                return CreatureStateMachine.CreatureStates.Feeding;
            case ActionManager.EPlayerAction.Attack:
                return CreatureStateMachine.CreatureStates.Attacking;
            default:
                return CreatureStateMachine.CreatureStates.Idle;
        }
    }
}
