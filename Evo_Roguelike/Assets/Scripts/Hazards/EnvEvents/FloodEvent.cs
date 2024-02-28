using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodEvent : EnvEventCommand
{
    public FloodEvent(int timestampToStop, int timestampToStart, GridManager gridManager, PopulationManager populationManager) : base(timestampToStop, timestampToStart, gridManager, populationManager)
    {

    }

    public FloodEvent(HazardFactory.EventParameters ep) : base(ep.timeToStop, ep.timeToStart, ep.gridManager, ep.populationManager)
    {

    }

    public override void EnvironmentEffect()
    {
        Debug.Log("Flood environmental effect");
        Vector3Int gridSize = _gridManager.GetGridSize();

        int randomX1 = Random.Range(-gridSize.x/2, gridSize.x/2);
        int randomY1 = Random.Range(-gridSize.y/2, gridSize.y/2);

        int randomX2 = Random.Range(-gridSize.x / 2, gridSize.x / 2);
        int randomY2 = Random.Range(-gridSize.y / 2, gridSize.y / 2);

        int smallerX = randomX1 < randomX2 ? randomX1 : randomX2;
        int largerX = randomX1 < randomX2 ? randomX2 : randomX1;

        int smallerY = randomY1 < randomY2 ? randomY1 : randomY2;
        int largerY = randomY1 < randomY2 ? randomY2 : randomY1;

        for(int i = smallerX; i <= largerX; i++)
        {
            _gridManager.ChangeGroundTile(new Vector3Int(i, smallerY), GroundTile.GroundTileType.Water);
        }

        for (int j = smallerY; j <= largerY; j++)
        {
            _gridManager.ChangeGroundTile(new Vector3Int(largerX, j), GroundTile.GroundTileType.Water);
        }
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
