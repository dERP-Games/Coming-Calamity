
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for managing hazards and executing them at the correct time.
/// </summary>
public class HazardManager
{
    // Passed in from Monobehaviour
    public HazardGenerationStrategy.Strategy generationStrategy;
    private TimeManager _timeManager;
    private GridManager _gridManager;
    private PopulationManager _populationManager;

    // Holds start time of hazard and list of hazard to be executed at that time
    public Dictionary<int, List<HazardCommand>> _hazardsToExectute;


    // Called when hazards are generated
    public delegate void DHazardsGenerated();
    public DHazardsGenerated dHazardsGenerated;

    public HazardManager(TimeManager timeManager, GridManager gridManager, PopulationManager population, HazardGenerationStrategy.Strategy generationStrategy)
    {
        this._timeManager = timeManager;
        this._gridManager = gridManager;
        this._populationManager = population;
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

    /// <summary>
    /// On new timestep, checks if any hazards need to be executed and executes them
    /// </summary>
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

    /// <summary>
    /// Creates strategy object based on enum and asks for list of hazards.
    /// </summary>
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

    /// <summary>
    /// Creates strategy object based on enum
    /// </summary>
    /// <param name="strategy"> Enum siginfying type of strategy to be created </param>
    /// <returns></returns>
    public HazardGenerationStrategy GetStrategyObjectFromEnum(HazardGenerationStrategy.Strategy strategy)
    {
        switch(strategy)
        {
            case (HazardGenerationStrategy.Strategy.SimpleRandom):
            {
                return new SimpleRandomHazardStrategy(_timeManager, _gridManager, _populationManager);
            }
            default:
                return null;
        }
    }

    #region QueryFunctions

    /// <summary>
    /// Logs current hazards in queue to the console
    /// </summary>
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

    /// <summary>
    /// Gets list of hazard at particular timestamp
    /// </summary>
    /// <param name="timestamp"> timestep at which hazards are to be exectuted at </param>
    /// <returns></returns>
    public List<HazardCommand> GetHazardsAtTimeStamp(int timestamp)
    {
        _hazardsToExectute.TryGetValue(timestamp, out List<HazardCommand> hazardsThisTick);
        return hazardsThisTick;
    }

    #endregion
}
