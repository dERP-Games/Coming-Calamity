using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager
{
    public HazardGenerationStrategy hazardGen;
    public List<HazardCommand> allHazards;
    public Dictionary<int, List<HazardCommand>> hazardsToExectute;

    private TimeManager _timeManager;
    
    

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

    public void Start()
    {
        GenerateHazards();
    }

    private void CustomTick()
    {
        bool bSuccess = hazardsToExectute.TryGetValue(_timeManager.CurrentTimeStep, out List<HazardCommand> hazardsThisTick);
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
        hazardGen.GenerateHazards();
    }
}
