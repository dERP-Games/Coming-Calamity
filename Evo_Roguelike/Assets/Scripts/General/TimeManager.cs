using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Responsible for keeping custom time step of game.
 */
public class TimeManager
{
    public delegate void D_Tick();
    public D_Tick D_tick;

    // From Monobehavior
    private float _timeStepDuration;

    private float _timeCounter;
    private int _currentTimeStep = 0;
    private bool _bIsPaused = false;

    // Properties
    public int CurrentTimeStep
    {
        get { return _currentTimeStep; }
    }
    public bool BIsPaused
    {
        get { return _bIsPaused; }
    }

    public TimeManager(float timeStepDuration)
    {
        _timeStepDuration = timeStepDuration;
        _timeCounter = _timeStepDuration;
    }

    /*
     * Keeps time based on deltaTime.
     * Input
     * deltaTime : time since last frame.
     */
    public void Tick(float deltaTime)
    {
        if (_bIsPaused) return;

        _timeCounter -= deltaTime;
        if (_timeCounter < 0f)
        {
            _currentTimeStep++;
            D_tick?.Invoke();
            _timeCounter = _timeStepDuration;
        }
    }

    public void PauseTimer()
    {
        _bIsPaused = true;
    }

    public void ResumeTimer()
    {
        _bIsPaused = false;
    }

    public void SetTimeStepDuration(float timeStepDuration)
    {
        _timeStepDuration = timeStepDuration;
    }
}
