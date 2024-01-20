using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodEvent : EnvEventCommand
{
    public FloodEvent(int timestampToStop, int timestampToStart, GridManager gridManager, PopulationManager populationManager) : base(timestampToStop, timestampToStart, gridManager, populationManager)
    {

    }

    public override void EnvironmentEffect()
    {
        Debug.Log("Flood environmental effect");
    }

    public override void PopulationEffect()
    {
        Debug.Log("Flood population effect");
    }

}
