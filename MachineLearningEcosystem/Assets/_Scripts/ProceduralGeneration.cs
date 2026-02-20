using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

// Much thanks to https://www.uproomgames.com/dev-log/wave-function-collapse for their example implementation of a WFC algorithm

public class ProceduralGeneration : MonoBehaviour
{
    // Using the procedural generation model of wave function collapse, this program is
    // designed to generate an x by y sized tilemap of different terrains based on
    // constraints for terrain placement
    private static int iter = 0;
    private static int maxIter;
    public static void CollapseWaveFunction(WFCCell[,] cells)
    {
        int rows = cells.GetLength(0);
        int columns = cells.GetLength(1);
        iter = 0;
        maxIter = rows * columns;
        bool fullyCollapsed = false;

        // the stack of unaddressed tiles
        Stack<Vector2Int> fringe = new Stack<Vector2Int>();
        // A set of the tiles already addressed to account for duplicates
        HashSet<Vector2Int> duplicateTiles = new HashSet<Vector2Int>();

        while (!fullyCollapsed)
        {
            if (iter >= maxIter)
            {
                fullyCollapsed = true;
                Debug.Log("Did not properly collapse!");
            }
            
            WFCCell cell = GetLowestEntropy(cells);
            if (cell.placement == new Vector2Int(-1, -1))
            {
                fullyCollapsed = true;
                Debug.Log("Finished collapsing");
            }
            else
            {
                cell.Collapse();
                PropagateChanges(cell, cells, fringe, duplicateTiles);
                iter++;
            }
        }
    }

    private static WFCCell GetLowestEntropy(WFCCell[,] cells)
    {
        float lowestEntropy = cells[0,0].GetEntropy();
        WFCCell targetCell = new WFCCell(-1, -1);
        foreach (WFCCell c in cells)
        {
            float noise = 0.0001f * Random.Range(0f, 1f);
            float entropy = c.GetEntropy();
            if (entropy + noise < lowestEntropy)
            {
                lowestEntropy = entropy + noise;
                targetCell = c;
            }
        }
        return targetCell;
    }

    private static void PropagateChanges(WFCCell cell, WFCCell[,] cells, Stack<Vector2Int> fringe, HashSet<Vector2Int> duplicateTiles)
    {
        List<WFCCell> neighbors = GetNeighbors(cell, cells);
        foreach (WFCCell c in neighbors)
        {
            fringe.Push(c.placement);
        }
        Vector2Int index;
        while (fringe.Count > 0)
        {
            index = fringe.Pop();
            cell = cells[index.x, index.y];

            bool changed = cell.ValidatePossibilitySpace(); // determine if tiles can be removed from the possible tiles

            duplicateTiles.Remove(index);
            if (cell.possibleTiles.Count == 1 && !cell.collapsed)
            {
                cell.Collapse();
            }

            if (changed)
            {
                neighbors = GetNeighbors(cell, cells);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int j = neighbors[i].placement;
                    if (!cells[j.x, j.y].collapsed && !duplicateTiles.Contains(neighbors[i].placement))
                    {
                        fringe.Push(neighbors[i].placement);
                        duplicateTiles.Add(neighbors[i].placement);
                    }
                }
            }
        }
    }

    private static List<WFCCell> GetNeighbors(WFCCell cell, WFCCell[,] cells)
    {
        Vector2Int pos = cell.placement;
        List<WFCCell> neighbors = new();
        if (InBounds(cells.GetLength(0), cells.GetLength(1), pos.x + 1, pos.y)) {
            neighbors.Add(cells[pos.x + 1, pos.y]);
        }
        if (InBounds(cells.GetLength(0), cells.GetLength(1), pos.x - 1, pos.y)) {
            neighbors.Add(cells[pos.x - 1, pos.y]);
        }
        if (InBounds(cells.GetLength(0), cells.GetLength(1), pos.x, pos.y + 1)) {
            neighbors.Add(cells[pos.x, pos.y + 1]);
        }
        if (InBounds(cells.GetLength(0), cells.GetLength(1), pos.x, pos.y - 1)) {
            neighbors.Add(cells[pos.x, pos.y - 1]);
        }
        return neighbors;
    }

    private static bool InBounds(int xDim, int yDim, int xCoord, int yCoord)
    {
        return (xCoord >= 0 && xCoord < xDim && yCoord >= 0 && yCoord < yDim);
    }
}
