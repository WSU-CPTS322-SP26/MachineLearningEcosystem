using System.Collections.Generic;
using UnityEngine;

public class WFCCell
{
    GameObject item;
    public List<MapTerrain> possibleTiles;
    private MapTerrain terrain;
    public bool collapsed;

    public void SetTerrain(MapTerrain t)
    {
        terrain = t;
    }
    public MapTerrain GetTerrain()
    {
        return terrain;
    }
}
