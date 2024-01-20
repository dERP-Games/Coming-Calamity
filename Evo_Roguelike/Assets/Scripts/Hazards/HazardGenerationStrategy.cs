using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardGenerationStrategy
{
    public abstract List<HazardCommand> GenerateHazards();

}
