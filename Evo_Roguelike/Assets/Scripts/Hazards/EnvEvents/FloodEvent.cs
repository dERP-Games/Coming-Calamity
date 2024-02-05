using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodEvent : EnvEventCommand
{
    public FloodEvent(int timestampToStop, int timestampToStart, GridManager gridManager, PopulationManager populationManager) : base(timestampToStop, timestampToStart, gridManager, populationManager)
    {

    }

    public FloodEvent(HazardFactory.EventParameters ep) : base(ep.timeToStop, ep.timeToStart, ep.gridManager, ep.populationManager)
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

    public override string ToString() 
    {
        return "Flood Event";
    }
}
