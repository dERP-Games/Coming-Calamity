using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all hazard generation strategies
/// </summary>
public abstract class HazardGenerationStrategy
{
    public enum Strategy
    {
        SimpleRandom,
        Scripted
    }

    protected TimeManager _timeManager;
    protected GridManager _gridManager;
    protected PopulationManager _populationManager;

    public HazardGenerationStrategy(TimeManager timeManager, GridManager gridManager, PopulationManager populationManager)
    {
        _timeManager = timeManager;
        _gridManager = gridManager;
        _populationManager = populationManager;
    }

    public abstract List<HazardCommand> GenerateHazards();

}
