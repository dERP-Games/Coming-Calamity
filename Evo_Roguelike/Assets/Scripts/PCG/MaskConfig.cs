using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MaskConfig
{
    [Tooltip("Determines whether mask preserves or reduces height")]
    public bool isNegative;

    [Tooltip("Type of the mask to be applied")]
    public MaskGeneratorType maskType;

    [Tooltip("Type of fader to use")]
    public FaderType faderType;

    [Tooltip("Determines the center of the mask")]
    public Vector2 center;

    [Tooltip("Size of the radius for radial masks, width for all others")]
    public float sizeVariable1; // Radius for radial masks, width for elliptical and rectangular ones

    [Tooltip("Height of the mask, unused in radial masks")]
    public float sizeVariable2; // Height for elliptical and rectangular masks
}

[CreateAssetMenu(menuName = "ScriptableObjects/MaskSetup")]
public class MaskSetup : ScriptableObject
{
    public MaskConfig[] maskConfig;
}
