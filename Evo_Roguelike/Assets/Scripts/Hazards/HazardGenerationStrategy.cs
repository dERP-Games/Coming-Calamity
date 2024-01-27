using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class HazardGenerationStrategy
{
    public enum Strategy
    {
        SimpleRandom,
        Scripted
    }

    public abstract List<HazardCommand> GenerateHazards();

}
