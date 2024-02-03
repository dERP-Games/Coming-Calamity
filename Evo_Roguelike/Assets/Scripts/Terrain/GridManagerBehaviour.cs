using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * This class is a service and will be accesible through the service locator.
 * It is a monobehavior wrapper for the GridManager class.
 */

[RequireComponent(typeof(TerrainGenerationManager))]
public class GridManagerBehaviour : MonoBehaviour
{
    
    [SerializeField]
    private Tilemap _groundTilemap;
    [SerializeField]
    private List<GroundTile> _groundTiles;
    [SerializeField]
    private NoiseQuantizer _noiseQuantizer;
    [SerializeField]
    private bool _bGenerateNewIslandOnGameStart;

    private GridManager _gridManager;
    private TerrainGenerationManager _terrainGenerationManager;


    public GridManager gridManager
    {
        get
        {
            if(_gridManager == null)
            {
                _terrainGenerationManager = GetComponent<TerrainGenerationManager>();
                _gridManager = new GridManager(_terrainGenerationManager, _groundTilemap, _groundTiles, _noiseQuantizer, _bGenerateNewIslandOnGameStart);
            }
            return _gridManager;
        }
    }


    void Start()
    {
        gridManager.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
