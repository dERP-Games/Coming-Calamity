using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Temporary UI element that shows current timestep
/// </summary>
public class TimeStepCounter : MonoBehaviour
{
    private TextMeshProUGUI _timeStepCounter;
    private TimeManager _timeManager;

    private TimeManager TimeManager
    {
        get { if(_timeManager == null)
            {
                _timeManager = FindObjectOfType<TimeManagerBehavior>().TimeManager;
            } 
        return _timeManager;
        }
    }

    private void Start()
    {
        
        _timeStepCounter = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        TimeManager.D_tick += OnTick;
    }

    private void OnDisable()
    {
        TimeManager.D_tick -= OnTick;
        
    }

    private void OnTick()
    {
        _timeStepCounter.text = "Time Step: " + _timeManager.CurrentTimeStep;
    }
}
