using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardGenerationStrategy : ScriptableObject
{
    public abstract List<HazardCommand> GenerateHazards();

}
