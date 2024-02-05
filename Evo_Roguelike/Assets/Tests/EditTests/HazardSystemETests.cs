using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class HazardSystemETests
{
    private int _hazardExecutionCounter = 0;
    /*private HazardManager InitializeHazardSystem(TimeHazardGenerationStrategy.Strategy genStrat)
    {
        TimeManager timeManager = new TimeManager(1f, true);
        HazardManager hazardManager = new HazardManager(timeManager, genStrat);

        return hazardManager;
    }*/

    private void IncreaseCounter()
    {
        _hazardExecutionCounter++;
    }

    [Test]
    public void TestHazardExecution()
    {
        TimeManager timeManager = new TimeManager(1f, true);
        HazardManager hazardManager = new HazardManager(timeManager, null, null, HazardGenerationStrategy.Strategy.SimpleRandom);

        hazardManager.OnEnable();
        hazardManager.Start();

        Dictionary<int, List<HazardCommand>> hazards = hazardManager._hazardsToExectute;
        _hazardExecutionCounter = 0;

        int expectedValue = 0;
        int highestStartTime = int.MinValue;
        foreach (KeyValuePair<int, List<HazardCommand>> kvp in hazards)
        {
            if(kvp.Key > highestStartTime)
            {
                highestStartTime = kvp.Key;
            }
            foreach(HazardCommand cmd in kvp.Value)
            {
                cmd.dHazardExecuted += IncreaseCounter;
                expectedValue++;
            }
        }

        
        for (int i = 0; i < highestStartTime; i++)
        {
            timeManager.AdvanceTimer();
        }

        Assert.AreEqual(expectedValue, _hazardExecutionCounter);
    }


}
