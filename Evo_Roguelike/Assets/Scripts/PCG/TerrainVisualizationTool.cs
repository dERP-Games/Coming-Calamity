using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainVisualizationTool : MonoBehaviour
{
    // This class exists for testing purposes of the PCG Test Scene..
    // It is _not_ intended to be used in production.It is used for visualization testing of the noise generators

    private Texture2D noiseTex;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        TerrainGenerationManager tgm = GetComponent<TerrainGenerationManager>();
        OutputToTexture(tgm.MakeNoiseValues());
    }

    private void OutputToTexture(float[,] noiseValues)
    {
        int mapWidth = noiseValues.GetLength(0);
        int mapHeight = noiseValues.GetLength(1);
        noiseTex = new Texture2D(mapWidth,mapHeight);
        rend = GetComponent<Renderer>();
        rend.material.mainTexture = noiseTex;
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                noiseTex.SetPixel(j, i, new Color(noiseValues[j, i], noiseValues[j, i], noiseValues[j, i]));
            }
        }
        noiseTex.Apply();
    }

}
