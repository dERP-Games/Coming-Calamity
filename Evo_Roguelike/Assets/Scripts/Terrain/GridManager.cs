using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * This class is responsible for managing the tilemap of the game.
 */
public class GridManager
{
    // Holds all the different tile types in the game
    private Dictionary<GroundTile.GroundTileType, GroundTile> _groundTilesDict;

    // Mapper between tile position and tile instance data
    private Dictionary<Vector3Int, GroundData> _groundDataDict;

    private TerrainGenerationManager _terrainGenerationManager;

    private Tilemap _groundTilemap;

    private List<GroundTile> _groundTiles;

    private NoiseQuantizer _noiseQuantizer = null;

    private bool _bGenerateNewIslandOnGameStart;

    public GridManager(TerrainGenerationManager terrainGenerationManager, Tilemap groundTilemap, List<GroundTile> groundTiles, NoiseQuantizer noiseQuantizer, bool bGenerateNewIslandOnGameStart)
    {
        /*
        * Takes in components necessary for calling PCG along with normal grid related componenets. Usually used in game.
        */
        this._terrainGenerationManager = terrainGenerationManager;
        this._groundTilemap = groundTilemap;
        this._groundTiles = groundTiles;
        this._noiseQuantizer = noiseQuantizer;
        this._bGenerateNewIslandOnGameStart = bGenerateNewIslandOnGameStart;
    }

    public GridManager(Tilemap groundTilemap, List<GroundTile> groundTiles)
    {
        /*
        * Only takes in grid related data, used for testing.
        */
        this._groundTilemap = groundTilemap;
        this._groundTiles = groundTiles;
    }

    public void Start()
    {
        if( _bGenerateNewIslandOnGameStart )
        {
            GenerateTileData();
        }
    }

    private void InitializeTiles()
    {
        /*
        * Initializes the tiles dictionary and tile data dictionary.
        */
        _groundTilesDict = new Dictionary<GroundTile.GroundTileType, GroundTile>();
        _groundDataDict = new Dictionary<Vector3Int, GroundData>();

        foreach (var tile in _groundTiles)
        {
            _groundTilesDict[tile.groundTileType] = tile;
        }
    }

    public void GenerateTileData()
    {
        /*
         * Calls for PCG terrain generation, translates into tile types, and sends data to be contstructed into tilemap.
         */
        float[,] noiseValues = _terrainGenerationManager.MakeNoiseValues();
        GenerateGroundTiles(_noiseQuantizer.GroundTilesFromNoise(noiseValues));
    }

    public void GenerateGroundTiles(GroundTile.GroundTileType[,] groundTiles)
    {
        /*
         * Generates tiles in tilemap based on data passed in. Called in-editor.
         * Input
         * tiles : 2D array of ints representing different tiles
         */

        if (_groundTilesDict == null || _groundTilesDict.Count == 0)
        {
            InitializeTiles();
        }

        // Temporarily removing Undo functionality as there is some possibility of members not resetting correctly on undo.
        //Undo.RecordObject(_groundTilemap, "Cleared Tiles");

        // Clearing tiles from tilemap and tile instance data.
        ClearTilemap();


        int halfWidth = groundTiles.GetLength(1) / 2;
        int halfHeight = groundTiles.GetLength(0) / 2;


        for (int i = 0; i < groundTiles.GetLength(0); i++)
        {
            for (int j = 0; j < groundTiles.GetLength(1); j++)
            {
                Vector3Int tilePos = new Vector3Int(-halfWidth + j, -halfHeight + i, 0);
                GroundTile.GroundTileType tileType = groundTiles[i, j];

                _groundTilemap.SetTile(tilePos, _groundTilesDict[tileType]);
                // Mapping tile position to its instance data.
                _groundDataDict[tilePos] = new GroundData(_groundTilemap.CellToWorld(tilePos), tileType);
            }
        }

        #if UNITY_EDITOR
            EditorUtility.SetDirty(_groundTilemap);
        #endif
    }

    public void ClearTilemap()
    {
        /*
         * Clears tiles from tilemap and resets instance data of tilemap.
         */
        if (_groundTilemap)
        {
            _groundTilemap.ClearAllTiles();

        }
        if (_groundDataDict != null)
        {
            _groundDataDict.Clear();
        }
    }


    /*
     * Query functions to be used from other parts of the game to get grid information.
     */
    #region Queries
    public GroundData GetGroundDataFromWorldPos(Vector3 worldPos)
    {
        /*
         * Gets tile instance information from world position.
         * Input
         * worldPos : point in world space
         */
        Vector3Int cellPos = _groundTilemap.WorldToCell(worldPos);
        return _groundDataDict[cellPos];
    }

    public GroundData GetGroundDataFromCellPos(Vector3Int cellPos)
    {
        /*
         * Gets tile instance information from cell position.
         * Input
         * cellPos : (x,y,z) coordinates of cell.
         */
        return _groundDataDict[cellPos];
    }


    public GroundTile.GroundTileType GetTileTypeFromWorldPos(Vector3 worldPos)
    {
        /*
         * Gets tile type from world position.
         * Input
         * worldPos : point in world space
         */
        Vector3Int cellPos = _groundTilemap.WorldToCell(worldPos);
        return _groundDataDict[cellPos].tileType;

    }

    public GroundTile.GroundTileType GetTileTypeFromCellPos(Vector3Int cellPos)
    {
        /*
         * Gets tile type from cell position.
         * Input
         * worldPos : (x,y,z) coordinates of cell.
         */
        return _groundDataDict[cellPos].tileType;

    }

    public void ChangeGroundTile(Vector3Int cellPos, GroundTile.GroundTileType newTileType)
    {
        GroundTile newGroundTile = _groundTilesDict[newTileType];
        _groundTilemap.SetTile(cellPos, newGroundTile);
        _groundDataDict[cellPos] = new GroundData(_groundTilemap.CellToWorld(cellPos), newTileType);
    }

    public Vector3Int GetGridSize()
    {
        return _groundTilemap.size;
    }

    #endregion

}