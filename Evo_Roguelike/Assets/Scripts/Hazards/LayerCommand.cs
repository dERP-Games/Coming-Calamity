using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LayerCommand : HazardCommand
{
    public LayerCommand(int timeToStart, GridManager gridManager, PopulationManager populationManager) : base(timeToStart, gridManager, populationManager)
    {

    }
}
