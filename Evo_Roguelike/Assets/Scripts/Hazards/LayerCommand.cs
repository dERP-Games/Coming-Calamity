using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all layer effec hazards
/// </summary>
public abstract class LayerCommand : HazardCommand
{
    public LayerCommand(int timeToStart, GridManager gridManager, PopulationManager populationManager) : base(timeToStart, gridManager, populationManager)
    {

    }
}
