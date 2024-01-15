using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum used for creature state machine
public enum CreatureStates
{
    Wandering,
    MovingTo,
    Avoiding,
    Death
}

/* CLASS: Creature
 * USAGE: Main hub for all creature components and controller for 
 * creature behavior via its state machine.
 */
public class Creature : MonoBehaviour
{
    // Public fields
    public CreatureStates currentState = CreatureStates.Wandering;
    public bool bIsActive = false;

    // Get needed components for handling creature behavior
    KinematicMovement _MovementControls;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the object
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // Check behavior when active
        if (bIsActive)
            CheckStateMachine();
    }

    /*
	USAGE: Initialize the object's fields and components
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void Init()
    {
        // Init component references
        _MovementControls = GetComponent<KinematicMovement>();
    }

    /*
	USAGE: Check the creature's state machine to decide agent's next behavior
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void CheckStateMachine()
    {
        // Switch on creature states
        // to control decision making
        switch (currentState)
        {
            // Handle wander behavior here
            case CreatureStates.Wandering:
                // Enable movement
                _MovementControls.CanMove = true;
                _MovementControls.IsWandering = true;
                break;

            // Handle target pursue behavior
            case CreatureStates.MovingTo:
                // TO-DO
                // Implement moving to behavior once environmental stimuli are implemented
                break;

            // Handle evasive behavior
            case CreatureStates.Avoiding:
                // TO-DO
                // Implement avoiding behavior once environmental stimuli are implemented
                break;

            // Handle creature death
            case CreatureStates.Death:
                // TO-DO
                // Implement death once vitals component is implemented
                break;
        }
    }
}
