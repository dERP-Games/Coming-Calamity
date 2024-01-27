using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SimpleRandomHazardStrategy : HazardGenerationStrategy
{
    private HazardFactory _hazardFactory = new HazardFactory();
    public override List<HazardCommand> GenerateHazards()
    {
        TimeManager timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
        GridManager gridManager = ServiceLocator.Instance.GetService<GridManagerBehaviour>().GridManager;

        List<HazardCommand> hazardCommands = new List<HazardCommand>();

        int curTimeStep = timeManager.CurrentTimeStep;

        for (int i = 0; i < 5; i++)
        {
            int eventStart = UnityEngine.Random.Range(curTimeStep + 1, curTimeStep + 10);
            int eventEnd = UnityEngine.Random.Range(eventStart + 1, eventStart + 5);

            HazardFactory.EventParameters ep = new HazardFactory.EventParameters(eventStart, eventEnd, gridManager, null) ;

            hazardCommands.Add(_hazardFactory.CreateEvent("flood", ep));
        }

        return hazardCommands;
    }

}
