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
public class GridManagerBehaviour : MonoBehaviour
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
    [SerializeField]
    private bool _bGenerateNewIslandOnGameStart;

    private GridManager _gridManager;

    public GridManager gridManager
    {
        get
        {
            if(_gridManager == null)
            {
                _gridManager = new GridManager(noiseType, maskType, faderType, terrainHeight,terrainWidth, _groundTilemap,
                    _groundTiles, _noiseQuantizer, _bGenerateNewIslandOnGameStart);
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
