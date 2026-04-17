using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

// Much thanks to https://www.uproomgames.com/dev-log/wave-function-collapse for their example implementation of a WFC algorithm

public class ProceduralGeneration : MonoBehaviour
{
    // Using the procedural generation model of wave function collapse, this program is
    // designed to generate an x by y sized tilemap of different terrains based on
    // constraints for terrain placement
    private static int animation_speed = 20;
    private static int max_animation_speed = 50;
    private static bool animated = true;
    private static int iter = 0;
    private static int maxIter;
    private static bool isGenerating = false;
    public static WFCCell[,] cells;

    public static IEnumerator CollapseWaveFunction(WFCCell[,] cs)
    {
        isGenerating = true;
        cells = cs;
        int rows = cells.GetLength(0);
        int columns = cells.GetLength(1);
        iter = 0;
        maxIter = rows * columns;
        bool fullyCollapsed = false;

        while (!fullyCollapsed)
        {
            if (!PauseButton.IsPaused()) 
            {       
                if (iter >= maxIter)
                {
                    Debug.Log("Error: Did not properly collapse!");
                    break;
                }
                
                WFCCell cell = GetLowestEntropy();
                if (cell.placement == new Vector2Int(-1, -1))
                {
                    fullyCollapsed = true;
                    //Debug.Log("Finished collapsing");
                }
                else
                {
                    cell.Collapse();
                    PropagateChanges(cell);
                    iter++;
                }
            }
            if (animated && iter % animation_speed == 0)
            {
                yield return null;
            }
        }
        isGenerating = false;
        yield return null;

        //Make PlantManager start after map is fully done Generating. 
        if (PlantManager.instance != null)
        {
            PlantManager.instance.Initialize();
        }
        yield return null;
    }

    private static WFCCell GetLowestEntropy()
    {
        float lowestEntropy = float.MaxValue;
        WFCCell targetCell = new WFCCell(-1, -1);
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                WFCCell c = cells[i, j];
                float noise = 0.0001f * Random.Range(-1f, 1f);
                float entropy = c.GetEntropy() + noise;
                if (entropy < lowestEntropy && !c.collapsed)
                {
                    lowestEntropy = entropy;
                    targetCell = c;
                }
            }
        }
        return targetCell;
    }

    private static void PropagateChanges(WFCCell cell)
    {
        // the stack of unaddressed tiles
        Queue<Vector2Int> fringe = new Queue<Vector2Int>();
        // A set of the tiles already addressed to account for duplicates
        HashSet<Vector2Int> duplicateTiles = new HashSet<Vector2Int>();
        // add neighbors of the collapsed cell to the fringe
        List<WFCCell> neighbors = GetNeighbors(cell);
        foreach (WFCCell c in neighbors)
        {
            fringe.Enqueue(c.placement);
        }
        Vector2Int index;
        while (fringe.Count > 0)
        {
            index = fringe.Dequeue();
            cell = cells[index.x, index.y];

            bool changed = cell.ValidatePossibilitySpace(GetNeighbors(cell)); // determine if tiles can be removed from the possible tiles

            duplicateTiles.Remove(index);
            if (cell.possibleTiles.Count == 1 && !cell.collapsed)
            {
                cell.Collapse();
            }

            if (changed)
            {
                neighbors = GetNeighbors(cell);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int neighborPlacement = neighbors[i].placement;
                    if (!cells[neighborPlacement.x, neighborPlacement.y].collapsed && !duplicateTiles.Contains(neighbors[i].placement))
                    {
                        fringe.Enqueue(neighbors[i].placement);
                        duplicateTiles.Add(neighbors[i].placement);
                    }
                }
            }
        }
    }

    public static List<WFCCell> GetNeighbors(WFCCell cell)
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
    private static void PrintCells(WFCCell[,] cells)
    {
        foreach (WFCCell c in cells)
        {
            Debug.Log(c.placement + ", " + c.GetTerrain());
        }
    }
    public static bool IsGenerating()
    {
        return isGenerating;
    }
    public static int GetAnimationSpeed()
    {
        return animation_speed;
    }
    public static int GetMaxAnimationSpeed()
    {
        return max_animation_speed;
    }
    public static bool GetAnimated()
    {
        return animated;
    }
    public static void SetAnimated(bool isAnimated)
    {
        animated = isAnimated;
    }
    public static void SetAnimationSpeed(int speed)
    {
        animation_speed = speed;
    }
    public static void ResetGeneration()
    {
        isGenerating = false;
        iter = 0;
        cells = null;
    }
}
