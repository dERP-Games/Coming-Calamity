using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Monobehavior wrapper for TimerManager class.
 */
public class TimeManagerBehavior : MonoBehaviour
{
    private TimeManager _timeManager;

    public TimeManager TimeManager
    {
        get {
            if (_timeManager == null)
            {
                _timeManager = new TimeManager(_timeStepDuration);
                return _timeManager;
            }

            else return _timeManager;
        }
    }

    [SerializeField]
    private float _timeStepDuration = 20f;

    void Update()
    {
        TimeManager.Tick(Time.deltaTime);
    }

}
