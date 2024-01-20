using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager
{
    private TimeManager _timeManager;
    private Dictionary<int, List<HazardCommand>> _hazardsToExectute;

    public HazardManager(TimeManager timeManager)
    {
        this._timeManager = timeManager;
    }

    public void OnEnable()
    {
        _timeManager.D_tick += CustomTick;
    }

    public void OnDisable()
    {
        _timeManager.D_tick -= CustomTick;
    }

    private void CustomTick()
    {
        bool bSuccess = _hazardsToExectute.TryGetValue(_timeManager.CurrentTimeStep, out List<HazardCommand> hazardsThisTick);
        if(bSuccess)
        {
            foreach(HazardCommand hazard in hazardsThisTick)
            {
                hazard.ExecuteEffects();
            }
        }

    }
    public void GenerateHazards()
    {

    }
}
