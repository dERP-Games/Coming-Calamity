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
    VitalityStatsComponent _VitalityStats;

    // Public get properties
    public CreatureStateMachine StateMachine
    {
        // Get state machine
        get { return _StateMachine; }
    }

    public VitalityStatsComponent VitalityStats
    {
        // Get vitality stats
        get { return _VitalityStats; }
    }


    // Start is called before the first frame update
    void Awake()
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

    /// <summary>
    /// Initialize the object's fields and components
    /// </summary>
    void Init()
    {
        // Init component references
        _StateMachine = GetComponent<CreatureStateMachine>();
        _VitalityStats = GetComponent<VitalityStatsComponent>();
    }
}
