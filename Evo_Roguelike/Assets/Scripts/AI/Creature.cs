using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureStates
{
    Wandering,
    MovingTo,
    Avoiding,
    Death
}

public class Creature : MonoBehaviour
{
    // Public fields
    public CreatureStates currentState = CreatureStates.Wandering;
    public bool bIsActive = false;

    // Get needed components for handling creature behavior
    public KinematicMovement movementControls;


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

    // Init method
    void Init()
    {
        // Init component references
        movementControls = GetComponent<KinematicMovement>();
    }

    // Method used for checking state machine
    void CheckStateMachine()
    {
        // Switch on creature states
        // to control decision making
        switch (currentState)
        {
            // Handle wander behavior here
            case CreatureStates.Wandering:
                // Enable movement
                movementControls.CanMove = true;
                movementControls.IsWandering = true;
                break;

            // Handle target pursue behavior
            case CreatureStates.MovingTo:
                break;

            // Handle evasive behavior
            case CreatureStates.Avoiding:
                break;

            // Handle creature death
            case CreatureStates.Death:
                break;
        }
    }
}
