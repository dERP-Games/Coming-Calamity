using UnityEngine;

public class TerrainGenerationManager : MonoBehaviour
{ 
    // This class exists as a Unity component that exposes the output of the noise generation classes.

    public NoiseGeneratorType noiseType;
    public MaskSetup maskSetup;
    public int mapHeight = 720;
    public int mapWidth = 720;
    private Generator _noiseGenerator;

    public float[,] MakeNoiseValues()
    {   
        _noiseGenerator = new Generator(new Vector2(mapWidth, mapHeight), noiseType, maskSetup);
        float[,] noiseValues = _noiseGenerator.GenerateNoiseArray(mapWidth, mapHeight);
        return noiseValues;        
    }
}
