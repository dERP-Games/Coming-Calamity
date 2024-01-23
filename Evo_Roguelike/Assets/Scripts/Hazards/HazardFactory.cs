using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardFactory
{
    public abstract HazardCommand CreateHazard();
}
