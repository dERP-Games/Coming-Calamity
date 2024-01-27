using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardFactory
{
    public struct EventParameters 
    {
        public int timeToStart;
        public int timeToStop;
        public GridManager gridManager;
        public PopulationManager populationManager;

        public EventParameters(int timeToStart, int timeToStop, GridManager gridManager, PopulationManager populationManager)
        {
            this.timeToStart = timeToStart;
            this.timeToStop = timeToStop;
            this.gridManager = gridManager;
            this.populationManager = populationManager;
        }
    }

    public HazardCommand CreateEvent(string hazardType, EventParameters ep)
    {
        switch(hazardType)
        {
            case "flood":
                return new FloodEvent(ep);
            default:
                {
                    Debug.Log("Invalid Event Type");
                    return null;
                }
        }
    }
}
