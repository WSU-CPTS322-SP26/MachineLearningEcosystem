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
            //Debug.Log("No possible tiles at " + placement + ", choosing random neighbor tile");
            List<WFCCell> neighbors = ProceduralGeneration.GetNeighbors(this);
            do
            {
                int rand = Random.Range(0, neighbors.Count);
                if (neighbors[rand].GetTerrain() != null)
                {
                    terrain = neighbors[rand].GetTerrain();
                    //Debug.Log("Random neighbor tile selected: " + terrain.GetTerrainType() + " at " + placement);
                    break;
                }
                else
                {
                    neighbors.RemoveAt(rand);
                }
            }
            while (neighbors.Count > 0);
            if (neighbors.Count == 0)
            {
                //Debug.Log("No neighbors with assigned tiles at " + placement + ", choosing totally random tile");
                var options = new List<MapTerrain>(MapManager.instance.GetTerrainOptions());
                terrain = options[Random.Range(0, options.Count)];
            }
        }
        else if (possibleTiles.Count == 1)
        {
            terrain = possibleTiles[0];
            //Debug.Log("Single tile option selected: " + terrain.GetTerrainType() + " from " + possibleTiles.Count + " options at " + placement);
            
        }
        else
        {
            int rand = Random.Range(0, possibleTiles.Count);
            terrain = possibleTiles[rand];
            //Debug.Log("Random tile option selected: " + terrain.GetTerrainType() + " from " + possibleTiles.Count + " options at " + placement);
        }
        collapsed = true;
        possibleTiles.Clear();
    }
    public bool ValidatePossibilitySpace(List<WFCCell> neighborCell)
    {
        bool changed = false;
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            MapTerrain possibleTile = possibleTiles[i];
            bool valid = true;
            foreach (WFCCell nc in neighborCell)
            {
                MapTerrain neighboringTerrain = nc.GetTerrain();
                if (neighboringTerrain != null && nc.collapsed && possibleTile.GetPossibleNS() != null)
                {
                    if (!possibleTile.GetPossibleNS().ContainsKey(neighboringTerrain.GetTerrainType()))
                    {
                        // selected possibleTile in array does not contain the neighber, so it is not an option to place there
                        valid = false;
                        break;
                    }
                    else
                    {
                        // selected possibleTile in array does contain the neighbor, update weights based on neighboringTerrain's dictionary
                        neighboringTerrain.GetPossibleNS().TryGetValue(possibleTile.GetTerrainType(), out float weightAddition);
                        possibleTile.SetWeight(possibleTile.GetWeight() + weightAddition);
                        Debug.Log("Adding weight of " + weightAddition + " to " + possibleTile.GetTerrainType() + " at " + placement + " based on neighbor " + neighboringTerrain.GetTerrainType());
                    }
                }
            }
            if (!valid)
            {
                possibleTiles.Remove(possibleTile);
                //Debug.Log("Removing " + t.GetTerrainType() + " from possible tiles at " + placement + " tiles remaining: " + possibleTiles.Count);
                i--;
                changed = true;
            }

        }
        return changed;
    }
    public void removePossibleTile(string t)
    {
        possibleTiles.Remove(possibleTiles.Find(tile => tile.GetTerrainType() == t));
    }
    public static WFCCell[,] DeepCopyCells(WFCCell[,] cells)
    {
        WFCCell[,] copy = new WFCCell[cells.GetLength(0), cells.GetLength(1)];
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                copy[i,j] = new WFCCell(cells[i,j]);
            }
        }
        return copy;
    }
}
