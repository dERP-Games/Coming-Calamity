using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingState : AbsBaseState<CreatureStateMachine.CreatureStates> 
{
    // Get movement controls
    KinematicMovement _MovementControls;

    // Constructor with call to base state class
    public WanderingState() : base(CreatureStateMachine.CreatureStates.Wandering)
    {}

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        _MovementControls = Owner.GetComponent<KinematicMovement>();
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
        _MovementControls.CanMove = true;
        _MovementControls.IsWandering = true;
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: CreatureStates, state to transition to given current game conditions
	*/
    public override CreatureStateMachine.CreatureStates GetNextState()
    {
        return CreatureStateMachine.CreatureStates.Wandering;
    }
}
