using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class handles per-instance data for ground tiles.
 * Any data that changes per tile or needs to be quickly accessed is stored here.
 * The linking between tiles and its per-instance data is done in the GridManager.
 */
public class GroundData
{
    public Vector3 worldPosition;
    public GroundTile.GroundTileType tileType;
    public float height;

    public GroundData(Vector3 worldPosition, GroundTile.GroundTileType tileType, float height)
    {
        this.worldPosition = worldPosition;
        this.tileType = tileType;
        this.height = height;
    }

}
