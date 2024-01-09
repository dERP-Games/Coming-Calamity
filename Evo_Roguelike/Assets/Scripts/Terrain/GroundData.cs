using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class handels per-instance data for ground tiles.
 * Any data that changes per tile or needs to be quickly accessed is stored here.
 * The linking between tiles and its per-instance data is done in the GridManager.
 */
public class GroundData
{
    public Vector3 worldPosition { get; private set; }
    public GroundTile.GroundTileType tileType { get; private set; }

    public GroundData(Vector3 worldPosition, GroundTile.GroundTileType tileType)
    {
        this.worldPosition = worldPosition;
        this.tileType = tileType;
    }

}
