using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap _groundTilemap;
    [SerializeField]
    private List<GroundTile> _groundTiles;

    private Dictionary<GroundTile.GroundTileType, GroundTile> _groundTilesDict;

    private Dictionary<Vector3Int, GroundData> _groundDataDict;

    void Start()
    {
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        _groundTilesDict = new Dictionary<GroundTile.GroundTileType, GroundTile> ();
        _groundDataDict = new Dictionary<Vector3Int, GroundData> ();

        foreach (var tile in _groundTiles)
        {
            _groundTilesDict[tile.groundTileType] = tile;
        }
    }

    public void GenerateGroundTiles(GroundTile.GroundTileType[,] tiles)
    {
        if(_groundTilesDict == null || _groundTilesDict.Count == 0)
        {
            InitializeTiles();
        }

        _groundTilemap.ClearAllTiles();
        _groundDataDict.Clear();

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                Vector3Int tilePos = new Vector3Int(j, i, 0);
                _groundTilemap.SetTile(tilePos, _groundTilesDict[tiles[i, j]]);
                _groundDataDict[tilePos] = new GroundData();
            }
        }
    }

    public  GroundTile.GroundTileType IntToGroundTileType(int val)
    {
        if (val < 0 || val >= _groundTiles.Count) return GroundTile.GroundTileType.None;
        
        return _groundTiles[val].groundTileType;
    }

#if UNITY_EDITOR
    public void GenerateGroundTilesEditor(int[,] tiles)
    {
        if (_groundTilesDict == null || _groundTilesDict.Count == 0)
        {
            InitializeTiles();
        }

        //var so = new SerializedObject(_groundTilemap);
        //Undo.RecordObject(_groundTilemap, "Cleared Tiles");

        _groundTilemap.ClearAllTiles();

        _groundDataDict.Clear();
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Vector3Int tilePos = new Vector3Int(j, i, 0);
                GroundTile.GroundTileType tileType = IntToGroundTileType(tiles[i, j]);
                _groundTilemap.SetTile(tilePos, _groundTilesDict[tileType]);
                _groundDataDict[tilePos] = new GroundData();
                EditorUtility.SetDirty(_groundTilemap);
            }
        }
    }
#endif 

}
