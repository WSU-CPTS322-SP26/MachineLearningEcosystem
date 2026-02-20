using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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
        terrain = null;
        collapsed = false;
    }
    public WFCCell(WFCCell c)
    {
        placement = c.placement;
        terrain = c.terrain;
        collapsed = c.collapsed;
        possibleTiles = new List<MapTerrain>(c.possibleTiles);
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
        if (possibleTiles.Count <= 0)
        {
            Debug.Log("No possible tiles at " + placement);
            ProceduralGeneration.UndoCollapse();
        }
        if (possibleTiles.Count == 1)
        {
            terrain = possibleTiles[0];
            Debug.Log("Single tile option selected: " + terrain.GetTerrainType() + " from " + possibleTiles.Count + " options at " + placement);
            
        }
        else
        {
            int rand = Random.Range(0, possibleTiles.Count);
            terrain = possibleTiles[rand];
            Debug.Log("Random tile option selected: " + terrain.GetTerrainType() + " from " + possibleTiles.Count + " options at " + placement);
        }
        collapsed = true;
        possibleTiles.Clear();
    }
    public bool ValidatePossibilitySpace(List<WFCCell> neighborType)
    {
        bool changed = false;
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            MapTerrain t = possibleTiles[i];
            bool valid = true;
            foreach (WFCCell n in neighborType)
            {
                if (n.GetTerrain() != null && n.collapsed && !t.GetPossibleNS().Contains(n.GetTerrain().GetTerrainType()))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                Debug.Log("Removing " + t.GetTerrainType() + " from possible tiles at " + placement);
                possibleTiles.Remove(t);
                i--;
                changed = true;
            }
        }
        if (!collapsed && possibleTiles.Count == 0)
        {
            Debug.Log("Contradiction at " + placement + ": no possible tiles remaining");
            ProceduralGeneration.UndoCollapse();
            changed = false;
        }
        return changed;
    }
    public void removePossibleTile(string t)
    {
        possibleTiles.Remove(possibleTiles.Find(tile => tile.GetTerrainType() == t));
    }
}
