using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class HazardCommand
{
    public int timestampToStart;

    protected GridManager _gridManager;
    protected PopulationManager _populationManager;

    public HazardCommand(int timestampToStart, GridManager gridManager, PopulationManager populationManager)
    {
        this.timestampToStart = timestampToStart;
        _gridManager = gridManager;
        _populationManager = populationManager;
    }

    public abstract void EnvironmentEffect();

    public abstract void PopulationEffect();


    public void ExecuteEffects()
    {
        EnvironmentEffect();
        PopulationEffect();
    }
}
