using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroundTileThreshold
{
    public GroundTile.GroundTileType tileType;
    public float minThreshold = 0f;
    public float maxThreshold = 1f;
    public bool bMaxInclusive = false;
}

[CreateAssetMenu(menuName = "ScriptableObjects/NoiseQuantizer")]
public class NoiseQuantizer : ScriptableObject
{
    public List<GroundTileThreshold> thresholds;


    public GroundTile.GroundTileType[,] GroundTilesFromNoise(float[,] tileData)
    {
        GroundTile.GroundTileType[,] groundTiles = new GroundTile.GroundTileType [tileData.GetLength(0), tileData.GetLength(1)];

        for(int i = 0; i < tileData.GetLength(0); i ++)
        {
            for(int j = 0; j < tileData.GetLength(1); j ++)
            {
                groundTiles[i,j] = GetTileType(tileData[i,j]);
            }
        }

        return groundTiles;
    }
    
    private GroundTile.GroundTileType GetTileType(float noiseVal)
    {
        foreach(GroundTileThreshold thresh in thresholds)
        {
            if(thresh.bMaxInclusive)
            {
                if(noiseVal >= thresh.minThreshold && noiseVal <= thresh.maxThreshold)
                {
                    return thresh.tileType;
                }
            }
            else
            {
                if (noiseVal >= thresh.minThreshold && noiseVal < thresh.maxThreshold)
                {
                    return thresh.tileType;
                }
            }
        }

        return GroundTile.GroundTileType.None;
    }
}
