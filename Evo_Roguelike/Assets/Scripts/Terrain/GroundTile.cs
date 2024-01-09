using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(menuName = "ScriptableObjects/GroundTile")]
public class GroundTile : TileBase
{
    public enum GroundTileType
    {
        None,
        Shallow,
        Water,
        Beach,
        Plain,
        Grassland,
        Forest,
        Mountain,
        Snow
    }

    public GroundTileType groundTileType = GroundTileType.None;

    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if(sprites.Count > 0)
        {
            tileData.sprite = sprites[0];
        }
    }
}
