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
    private struct Snapshot
    {
        public WFCCell decision;
        public string decidedTile;
        public WFCCell[,] cells;
    }

    // Using the procedural generation model of wave function collapse, this program is
    // designed to generate an x by y sized tilemap of different terrains based on
    // constraints for terrain placement
    private static int iter = 0;
    private static int maxIter;
    private static WFCCell[,] cells;
    // Stack is snapshots
    private static Stack<Snapshot> history = new();

    public static void CollapseWaveFunction(WFCCell[,] cs)
    {
        cells = cs;
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
            
            WFCCell cell = GetLowestEntropy();
            if (cell.placement == new Vector2Int(-1, -1))
            {
                fullyCollapsed = true;
                Debug.Log("Finished collapsing");
                history.Clear();
            }
            else
            {
                var s = new Snapshot { decision = cell, decidedTile = cell.possibleTiles[0].GetTerrainType(), cells = WFCCell.DeepCopyCells(cells) };
                cell.Collapse();
                s.decidedTile = cell.GetTerrain().GetTerrainType();
                history.Push(s);
                PropagateChanges(cell);
                iter++;
            }
        }
    }

    private static WFCCell GetLowestEntropy()
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
                var s = new Snapshot { decision = cell, decidedTile = cell.possibleTiles[0].GetTerrainType(), cells = WFCCell.DeepCopyCells(cells) };
                cell.Collapse();
                s.decidedTile = cell.GetTerrain().GetTerrainType();
                history.Push(s);
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

    private static List<WFCCell> GetNeighbors(WFCCell cell)
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
            Snapshot historySnapshot = history.Pop();
            cells = historySnapshot.cells;
            Vector2Int placement = historySnapshot.decision.placement;
            cells[placement.x, placement.y].removePossibleTile(historySnapshot.decidedTile);
        }
    }
}
