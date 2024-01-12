using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: CreatureStateMachine
 * USAGE: State Machine used for managing creature behavior and
 * storing all state keys.
 */
public class CreatureStateMachine : AbsStateMachine<CreatureStateMachine.CreatureStates>
{
    // Enum used for holding all creature state keys
    public enum CreatureStates
    {
        Wandering,
        MovingTo,
        Avoiding,
        Death
    }

    // Get needed components for state machine
    KinematicMovement _MovementControls;

    // Public properties for creature components
    public KinematicMovement MovementControls
    {
        get { return _MovementControls; }
    }

    void Awake()
    {
        // Initialize all components
        Init();

        // Fill the dictionary with needed states
        states[CreatureStates.Wandering] = new WanderingState();

        // TO-DO
        // Add other states to dictionary as they are implemented

        // Default state will be wandering
        currentState = states[CreatureStates.Wandering];
    }

    /*
	USAGE: Initialize the state machine's fields and components
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void Init()
    {
        _MovementControls = GetComponent<KinematicMovement>();
    }
}
