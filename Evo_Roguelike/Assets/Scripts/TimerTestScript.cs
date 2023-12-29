using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Temporary test script for testing timer and service locator functionality
 * Will be deleted after pr is approved before merging
 */

public class TimerTestScript : MonoBehaviour
{
    private TimeManagerBehavior timeManagerBehavior;

    private bool _bPauseFlag = true;

    private void OnEnable()
    {
        timeManagerBehavior = ServiceLocator.Instance.GetService<TimeManagerBehavior>();
        if (timeManagerBehavior != null)
        {
            timeManagerBehavior.TimeManager.D_tick += OnTick;
        }    
    }

    private void OnDisable()
    {
        if (timeManagerBehavior != null)
        {
            timeManagerBehavior.TimeManager.D_tick -= OnTick;
        }
    }

    private void Start()
    {
       
    }
    private void OnTick()
    {
        Debug.Log("New Tick");
    }

    private void Update()
    {
        // Using old input system just for convience, is temporary.
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (_bPauseFlag) timeManagerBehavior.TimeManager.PauseTimer();
            else timeManagerBehavior.TimeManager.ResumeTimer();

            _bPauseFlag = !_bPauseFlag;
        }  
    }
}
