using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Monobehavior wrapper for TimerManager class.
 */
public class TimeManagerBehavior : MonoBehaviour
{

    [SerializeField]
    private float _timeStepDuration = 20f;
    [SerializeField, Tooltip("If manual, timer step duration is irrelevant and timer needs to be told when to go to next timestep.")]
    private bool _bIsManual = true;

    private TimeManager _timeManager;

    public TimeManager TimeManager
    {
        get {
            if (_timeManager == null)
            {
                _timeManager = new TimeManager(_timeStepDuration, _bIsManual);
                return _timeManager;
            }

            else return _timeManager;
        }
    }

    void Update()
    {
        TimeManager.Tick(Time.deltaTime);
    }

    public void AdvanceTimer()
    {
        TimeManager.AdvanceTimer();
    }

}
