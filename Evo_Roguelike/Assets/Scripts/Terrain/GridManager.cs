using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * This class is a service and will be accesible through the service locator.
 * It is responsible for managing the tilemap of the game.
 */
[RequireComponent(typeof(TerrainGenerationManager))]
public class GridManager : MonoBehaviour
{
    public NoiseGeneratorType noiseType = NoiseGeneratorType.Default;
    public MaskGeneratorType maskType = MaskGeneratorType.Default;
    public FaderType faderType = FaderType.Default;
    public int terrainWidth = 720;
    public int terrainHeight = 720;

    [SerializeField]
    private Tilemap _groundTilemap;
    [SerializeField]
    private List<GroundTile> _groundTiles;
    [SerializeField]
    private NoiseQuantizer _noiseQuantizer;

    // Holds all the different tile types in the game
    private Dictionary<GroundTile.GroundTileType, GroundTile> _groundTilesDict;

    // Mapper between tile position and tile instance data
    private Dictionary<Vector3Int, GroundData> _groundDataDict;


    void Start()
    {
        InitializeTiles();
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
        TerrainGenerationManager tgm = GetComponent<TerrainGenerationManager>();
        float[,] noiseValues = tgm.MakeNoiseValues();
        GenerateGroundTilesEditor(_noiseQuantizer.GroundTilesFromNoise(noiseValues));
    }

    public void GenerateGroundTiles(GroundTile.GroundTileType[,] groundTiles)
    {
        /*
         * Generates tiles in tilemap based on data passed in. Called in-game.
         * Input
         * tiles : 2D array of ints representing different tiles
         */

        if (_groundTilesDict == null || _groundTilesDict.Count == 0)
        {
            InitializeTiles();
        }

        // Clearing tiles from tilemap and tile instance data.
        _groundTilemap.ClearAllTiles();
        _groundDataDict.Clear();

        for (int i = 0; i < groundTiles.GetLength(0); i++)
        {
            for (int j = 0; j < groundTiles.GetLength(1); j++)

            {
                Vector3Int tilePos = new Vector3Int(j, i, 0);
                GroundTile.GroundTileType tileType = groundTiles[i, j];

                _groundTilemap.SetTile(tilePos, _groundTilesDict[tileType]);
                // Mapping tile position to its instance data.
                _groundDataDict[tilePos] = new GroundData(_groundTilemap.CellToWorld(tilePos), tileType);
            }
        }
    }

#if UNITY_EDITOR
    public void GenerateGroundTilesEditor(GroundTile.GroundTileType[,] groundTiles)
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
        _groundTilemap.ClearAllTiles();
        _groundDataDict.Clear();


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

        EditorUtility.SetDirty(_groundTilemap);
    }
#endif




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
    #endregion

}