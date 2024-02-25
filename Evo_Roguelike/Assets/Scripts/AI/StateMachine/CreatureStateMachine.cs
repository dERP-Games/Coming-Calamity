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
        Idle,
        Wandering,
        MovingTo,
        Feeding,
        Attacking,
        Death
    }

    // Public fields
    public CreatureStates initialState = CreatureStates.Wandering;

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
        states[CreatureStates.Idle] = new IdleState();
        states[CreatureStates.Wandering] = new WanderingState();
        states[CreatureStates.MovingTo] = new MovingState();
        states[CreatureStates.Feeding] = new FeedingState();
        states[CreatureStates.Attacking] = new AttackingState();
        states[CreatureStates.Death] = new DeathState();

        // Default state will be wandering
        currentState = states[initialState];
    }

    /// <summary>
    /// Initialize the state machine's fields and components
    /// </summary>
    void Init()
    {
        _MovementControls = GetComponent<KinematicMovement>();
    }
}
