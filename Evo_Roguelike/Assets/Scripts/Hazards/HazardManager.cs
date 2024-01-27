
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager
{
    public HazardGenerationStrategy.Strategy generationStrategy;

    public Dictionary<int, List<HazardCommand>> _hazardsToExectute;
    private TimeManager _timeManager;

    public delegate void DHazardsGenerated();
    public DHazardsGenerated dHazardsGenerated;

    public HazardManager(TimeManager timeManager, HazardGenerationStrategy.Strategy generationStrategy)
    {
        this._timeManager = timeManager;
        this.generationStrategy = generationStrategy;
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
        _hazardsToExectute = new Dictionary<int, List<HazardCommand>>();

        HazardGenerationStrategy hazardGen = GetStrategyObjectFromEnum(generationStrategy);
        foreach(HazardCommand hazard in hazardGen.GenerateHazards())
        {
            bool bSuccess = _hazardsToExectute.TryGetValue(hazard.timestampToStart, out List<HazardCommand> hazardsThisTick);
            if(bSuccess)
            {
                _hazardsToExectute[hazard.timestampToStart].Add(hazard);
            }
            else
            {
                _hazardsToExectute.Add(hazard.timestampToStart, new List<HazardCommand> { hazard });
            }
        }

        dHazardsGenerated?.Invoke();
    }

    public HazardGenerationStrategy GetStrategyObjectFromEnum(HazardGenerationStrategy.Strategy strategy)
    {
        switch(strategy)
        {
            case (HazardGenerationStrategy.Strategy.SimpleRandom):
            {
                return new SimpleRandomHazardStrategy();
            }
            default:
                return null;
        }
    }

    #region QueryFunctions
    public void LogHazards()
    {
        foreach (KeyValuePair<int, List<HazardCommand>> kvp in _hazardsToExectute)
        {
            Debug.Log(kvp.Key);
            foreach(HazardCommand h in  kvp.Value)
            {
                Debug.Log(h.ToString());
            }
        }
    }

    public List<HazardCommand> GetHazardsAtTimeStamp(int timestamp)
    {
        _hazardsToExectute.TryGetValue(timestamp, out List<HazardCommand> hazardsThisTick);
        return hazardsThisTick;
    }

    public void ChangeStrategy(HazardGenerationStrategy.Strategy strategy)
    {
        Debug.Log("Changing strategy");
        this.generationStrategy = strategy;
    }
    #endregion
}
