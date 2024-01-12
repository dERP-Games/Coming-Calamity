using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Specifies what threshold of values gets translated to which tile type.
 */
[Serializable]
public class GroundTileThreshold
{
    public GroundTile.GroundTileType tileType;
    public float minThreshold = 0f;
    public float maxThreshold = 1f;
    public bool bMaxInclusive = false;
}

/*
 * Translates noise values into ground tiles using a list of GroudnTileThresholds
 */
[CreateAssetMenu(menuName = "ScriptableObjects/NoiseQuantizer")]
public class NoiseQuantizer : ScriptableObject
{
    public List<GroundTileThreshold> thresholds;


    /*
     * Converts 2D noise data into GroundTileTypes based on the thresholds specificed.
     * Input
     * tileData : 2D array of noise data representing terrain data.
     * Output
     * 2D array of GroundTileTypes representing the tiles in the tilemap.
     */
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

    /*
     * Converts a single noise value to a GroundTileType
     * Input
     * noiseVal : Singular noise value representing a single tile
     * Output
     * A singular GroundTileType
     */
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
