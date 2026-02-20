using System.Collections.Generic;
using UnityEngine;

public class WFCCell
{
    public Vector2Int placement;
    public List<MapTerrain> possibleTiles;
    private MapTerrain terrain;
    public bool collapsed;
    private float entropy;

    public WFCCell(int x, int y)
    {
        placement = new Vector2Int(x,y);
    }
    public void SetTerrain(MapTerrain t)
    {
        terrain = t;
    }
    public MapTerrain GetTerrain()
    {
        return terrain;
    }
    public float GetEntropy()
    {
        // calculates then returns
        entropy = possibleTiles.Count;
        return entropy;
    }
    public void Collapse()
    {
        if (possibleTiles.Count == 1)
        {
            terrain = possibleTiles[0];
        }
        else
        {
            terrain = possibleTiles[Random.Range(0, possibleTiles.Count)];
        }
    }
    public bool ValidatePossibilitySpace()
    {
        return true;
    }
}
