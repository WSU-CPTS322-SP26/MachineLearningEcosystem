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
    private static int iter = 0;
    private static int maxIter;
    // Stack is: Old state copy, chosen terrain, cell reference
    private static Stack<Tuple<WFCCell, string, WFCCell>> history = new(); // history of collapsed cells for dealing with contradictions
    public static void CollapseWaveFunction(WFCCell[,] cells)
    {
        int rows = cells.GetLength(0);
        int columns = cells.GetLength(1);
        iter = 0;
        maxIter = rows * columns;
        bool fullyCollapsed = false;

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
                history.Clear();
            }
            else
            {
                WFCCell cellCopy = new(cell);
                cell.Collapse();
                Tuple<WFCCell, string, WFCCell> cellState = new(cellCopy, cell.GetTerrain().GetTerrainType(), cell);
                history.Push(cellState);
                PropagateChanges(cell, cells);
                iter++;
            }
        }
    }

    private static WFCCell GetLowestEntropy(WFCCell[,] cells)
    {
        float lowestEntropy = float.MaxValue;
        WFCCell targetCell = new WFCCell(-1, -1);
        foreach (WFCCell c in cells)
        {
            float noise = 0.00000000000001f * Random.Range(0f, 1f);
            float entropy = c.GetEntropy() + noise;
            if (entropy < lowestEntropy && c.GetEntropy() != 0f)
            {
                lowestEntropy = entropy;
                targetCell = c;
            }
        }
        return targetCell;
    }

    private static void PropagateChanges(WFCCell cell, WFCCell[,] cells)
    {
        // the stack of unaddressed tiles
        Queue<Vector2Int> fringe = new Queue<Vector2Int>();
        // A set of the tiles already addressed to account for duplicates
        HashSet<Vector2Int> duplicateTiles = new HashSet<Vector2Int>();
        // add neighbors of the collapsed cell to the fringe
        List<WFCCell> neighbors = GetNeighbors(cell, cells);
        foreach (WFCCell c in neighbors)
        {
            fringe.Enqueue(c.placement);
        }
        Vector2Int index;
        while (fringe.Count > 0)
        {
            index = fringe.Dequeue();
            cell = cells[index.x, index.y];

            bool changed = cell.ValidatePossibilitySpace(GetNeighbors(cell, cells)); // determine if tiles can be removed from the possible tiles

            duplicateTiles.Remove(index);
            if (cell.possibleTiles.Count == 1 && !cell.collapsed)
            {
                WFCCell cellCopy = new(cell);
                cell.Collapse();
                Tuple<WFCCell, string, WFCCell> cellState = new(cellCopy, cell.GetTerrain().GetTerrainType(), cell);
                history.Push(cellState);
            }

            if (changed)
            {
                neighbors = GetNeighbors(cell, cells);
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
    private static void PrintCells(WFCCell[,] cells)
    {
        foreach (WFCCell c in cells)
        {
            Debug.Log(c.placement + ", " + c.GetTerrain());
        }
    }
    public static void UndoCollapse()
    {
        if (history.Count > 0)
        {
            Tuple<WFCCell, string, WFCCell> lastCell = history.Pop();
            WFCCell oldCell = lastCell.Item1;
            string terrainType = lastCell.Item2;
            WFCCell cellRef = lastCell.Item3;
            cellRef.possibleTiles = new List<MapTerrain>(oldCell.possibleTiles);
            cellRef.removePossibleTile(terrainType);
            cellRef.collapsed = false;
            cellRef.SetTerrain(null);

            Debug.Log("Redoing collapse at " + cellRef.placement + " of terrain type " + terrainType);
            WFCCell cellCopy = new(cellRef);
            cellRef.Collapse();
            Tuple<WFCCell, string, WFCCell> cellState = new(cellCopy, cellRef.GetTerrain().GetTerrainType(), cellRef);
            history.Push(cellState);
            Debug.Log("Redo complete at " + cellRef.placement + " of terrain type " + terrainType);
            PropagateChanges(cellRef, MapManager.instance.GetCells());
        }
    }
}
