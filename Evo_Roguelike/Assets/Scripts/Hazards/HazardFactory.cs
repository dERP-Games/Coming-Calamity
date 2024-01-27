using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for creating HazardCommands
/// Currently a concrete class but may be refactored to be abstract with more specialized children handling creation in the future
/// </summary>
public class HazardFactory
{
    /// <summary>
    /// Contains general parameters to create a event
    /// </summary>
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

    /// <summary>
    /// Creates an event based on type and parametrs passed in
    /// </summary>
    /// <param name="hazardType">Type of event to be created</param>
    /// <param name="ep">Parameters needed to create an event</param>
    /// <returns> Requested HazardCommand object </returns>
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
