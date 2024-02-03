using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingState : AbsBaseState<CreatureStateMachine.CreatureStates>
{
    // Get movement controls
    KinematicMovement _MovementControls;

    // Constructor with call to base state class
    public FeedingState() : base(CreatureStateMachine.CreatureStates.Feeding)
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
        // TO-DO
        // Implement feed behavior here
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: CreatureStates, state to transition to given current game conditions
	*/
    public override CreatureStateMachine.CreatureStates GetNextState()
    {
        return CreatureStateMachine.CreatureStates.Feeding;
    }
}
