using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvEventCommand : HazardCommand
{
    protected int _timestampToStop = -1;

    public EnvEventCommand(int timestampToStop, int timestampToStart, GridManager gridManager, PopulationManager populationManager) : base(timestampToStart, gridManager, populationManager)
    {
        _timestampToStop = timestampToStop;
    }
}
