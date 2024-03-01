using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using UnityEditor;

/*
 * Play Mode tests for the grid system
 */
public class GridSystemPTests
{
    /*
     * Creates a new GridManager instances
     * Output:
     * GridManager object
     */
    private GridManager CreateGridManagerInstance()
    {
        GameObject go = new GameObject();
        Grid grid = go.AddComponent<Grid>();
        GameObject tileGO = new GameObject();
        tileGO.transform.parent = go.transform;
        Tilemap groundTilemap = tileGO.AddComponent<Tilemap>();
        GroundTile beach = (GroundTile)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Terrain/Tiles/BeachTile.asset", typeof(GroundTile));
        GroundTile water = (GroundTile)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Terrain/Tiles/WaterTile.asset", typeof(GroundTile));
        GroundTile forest = (GroundTile)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Terrain/Tiles/ForestTile.asset", typeof(GroundTile));
        GroundTile snow = (GroundTile)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Terrain/Tiles/SnowTile.asset", typeof(GroundTile));
        List<GroundTile> groundTiles = new List<GroundTile> { beach, water, forest, snow };
        GridManager gridManager = new GridManager(groundTilemap, groundTiles);
        return gridManager;
    }


    [UnityTest]
    public IEnumerator TestTilemapGeneration()
    {
        /*
         * Testing tilemap generation with a 2x2 grid.
         */
        GridManager gridManager = CreateGridManagerInstance();
        GroundTile.GroundTileType[,] tileTypes = { { GroundTile.GroundTileType.Beach, GroundTile.GroundTileType.Water }, { GroundTile.GroundTileType.Forest, GroundTile.GroundTileType.Snow } };
        float[,] noiseValues = { { 0.1f,0.2f}, {0.3f, 0.4f } };
        gridManager.GenerateGroundTiles(tileTypes, noiseValues);

        GroundTile.GroundTileType expected1 = GroundTile.GroundTileType.Beach;
        GroundTile.GroundTileType expected2 = GroundTile.GroundTileType.Water;
        GroundTile.GroundTileType expected3 = GroundTile.GroundTileType.Forest;
        GroundTile.GroundTileType expected4 = GroundTile.GroundTileType.Snow;

        yield return null;

        Assert.AreEqual(expected1, gridManager.GetTileTypeFromWorldPos(new Vector3(-0.5f, -0.5f, 0f)));
        Assert.AreEqual(expected2, gridManager.GetTileTypeFromWorldPos(new Vector3(0.5f, -0.5f, 0f)));
        Assert.AreEqual(expected3, gridManager.GetTileTypeFromWorldPos(new Vector3(-0.5f, 0.5f, 0f)));
        Assert.AreEqual(expected4, gridManager.GetTileTypeFromWorldPos(new Vector3(0.5f, 0.5f, 0f)));
    }

    [UnityTest]
    public IEnumerator TestGroundData()
    {
        /*
         * Testing instance tile data.
         */
        GridManager gridManager = CreateGridManagerInstance();
        GroundTile.GroundTileType[,] tileTypes = { { GroundTile.GroundTileType.Beach, GroundTile.GroundTileType.Water }, { GroundTile.GroundTileType.Forest, GroundTile.GroundTileType.Snow } };
        float[,] noiseValues = { { 0.1f, 0.2f }, { 0.3f, 0.4f } };
        gridManager.GenerateGroundTiles(tileTypes, noiseValues);

        GroundData groundData = gridManager.GetGroundDataFromWorldPos(new Vector3(-0.5f, -0.5f, 0f));
        groundData.worldPosition = new Vector3(-5, -5, 0);

        Vector3 expected1 = new Vector3(-5, -5, 0);
        Vector3 expected2 = new Vector3(0, -1, 0);

        yield return null;

        Assert.AreEqual(expected1, gridManager.GetGroundDataFromWorldPos(new Vector3(-0.5f, -0.5f, 0f)).worldPosition);
        Assert.AreEqual(expected2, gridManager.GetGroundDataFromWorldPos(new Vector3(0.5f, -0.5f, 0f)).worldPosition);

    }
}
