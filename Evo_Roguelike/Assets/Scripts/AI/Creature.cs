using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: Creature
 * USAGE: Main hub for all creature components and controller for 
 * creature behavior via its state machine.
 */
public class Creature : MonoBehaviour
{
    // Public fields
    public bool bIsActive = false;

    // Get reference to creature's state machine
    CreatureStateMachine _StateMachine;

    public CreatureStateMachine StateMachine
    {
        // Get state machine
        get { return _StateMachine; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the object
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // Feed state machine whether creature is active
        _StateMachine.bIsActive = bIsActive;
    }

    /*
	USAGE: Initialize the object's fields and components
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void Init()
    {
        // Init component references
        _StateMachine = GetComponent<CreatureStateMachine>();
    }
}
