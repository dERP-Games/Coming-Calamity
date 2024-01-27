using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// Creates a list of hazards based on simple random logic
/// </summary>
public class SimpleRandomHazardStrategy : HazardGenerationStrategy
{
    private HazardFactory _hazardFactory = new HazardFactory();


    public SimpleRandomHazardStrategy(TimeManager timeManager, GridManager gridManager, PopulationManager populationManager) : base(timeManager, gridManager, populationManager)
    { 

    }


    /// <summary>
    /// Generates a list of hazards based on simple random logic
    /// </summary>
    /// <returns> List of HazardCommand objects </returns>
    public override List<HazardCommand> GenerateHazards()
    {

        List<HazardCommand> hazardCommands = new List<HazardCommand>();

        int curTimeStep = _timeManager.CurrentTimeStep;

        for (int i = 0; i < 5; i++)
        {
            int eventStart = UnityEngine.Random.Range(curTimeStep + 1, curTimeStep + 10);
            int eventEnd = UnityEngine.Random.Range(eventStart + 1, eventStart + 5);

            HazardFactory.EventParameters ep = new HazardFactory.EventParameters(eventStart, eventEnd, _gridManager, _populationManager) ;

            hazardCommands.Add(_hazardFactory.CreateEvent("flood", ep));
        }

        return hazardCommands;
    }

}
