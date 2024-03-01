using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloodEvent : EnvEventCommand
{
    public FloodEvent(int timestampToStop, int timestampToStart, GridManager gridManager, PopulationManager populationManager) : base(timestampToStop, timestampToStart, gridManager, populationManager)
    {

    }

    public FloodEvent(HazardFactory.EventParameters ep) : base(ep.timeToStop, ep.timeToStart, ep.gridManager, ep.populationManager)
    {

    }

    /// <summary>
    /// Effect of event on environment.
    /// </summary>
    public override void EnvironmentEffect()
    {
        if (_gridManager == null) return;

        Debug.Log("Flood environmental effect");
        Vector3Int gridSize = _gridManager.GetGridSize();

        int randomX1 = Random.Range(-gridSize.x/2, gridSize.x/2);
        int randomY1 = Random.Range(-gridSize.y/2, gridSize.y/2);

        Vector3Int startPoint = new Vector3Int(randomX1, randomY1, 0);

        int randomX2 = Random.Range(-gridSize.x / 2, gridSize.x / 2);
        int randomY2 = Random.Range(-gridSize.y / 2, gridSize.y / 2);

        Vector3Int endPoint = new Vector3Int(randomX2, randomY2, 0);
        Vector3Int curPoint = startPoint;

        while(curPoint != endPoint)
        {
            Debug.Log(curPoint);
            _gridManager.ChangeGroundTile(curPoint, GroundTile.GroundTileType.Water);

            Vector3 floodDir = (endPoint - curPoint);
            floodDir.Normalize();


            if(Mathf.Abs(floodDir.x) > Mathf.Abs(floodDir.y))
            {
                curPoint = curPoint + new Vector3Int((int) Mathf.Sign(floodDir.x), 0, 0);
            }
            else
            {
                curPoint = curPoint + new Vector3Int(0, (int)Mathf.Sign(floodDir.y), 0);
            }
        }

        _gridManager.ChangeGroundTile(curPoint, GroundTile.GroundTileType.Water);

    }

    public override void PopulationEffect()
    {
        Debug.Log("Flood population effect");
    }

    public override string ToString() 
    {
        return "Flood Event";
    }
}
