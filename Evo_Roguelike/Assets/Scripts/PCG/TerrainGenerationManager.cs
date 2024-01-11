using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationManager : MonoBehaviour
{ 
    // This class exists for testing purposes of the PCG Test Scene.
    // It is _not_ intended to be used in production. It is used for visualization testing of the noise generators

    public NoiseGeneratorType noiseType;
    public MaskSetup maskSetup;

    public Generator noiseGenerator;

    private Texture2D noiseTex;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        int height = 720;
        int width = 720;
        noiseTex = new Texture2D(width, height);
        rend = GetComponent<Renderer>();
        rend.material.mainTexture = noiseTex;

        noiseGenerator = new Generator(noiseType, maskSetup);
        float[,] noiseValues = noiseGenerator.GenerateNoiseArray(width, height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                noiseTex.SetPixel(j, i, new Color(noiseValues[j, i], noiseValues[j, i], noiseValues[j, i]));
            }
        }

        noiseTex.Apply();
    }

    
}
