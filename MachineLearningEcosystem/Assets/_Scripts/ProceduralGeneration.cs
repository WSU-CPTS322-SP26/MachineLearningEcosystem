using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    // Using the procedural generation model of wave function collapse, this program is
    // designed to generate an x by y sized tilemap of different terrains based on
    // constraints for terrain placement
    private static int iter = 0;
    private static int maxIter;
    public static void CollapseWaveFunction(WFCCell[,] map, List<Terrain> options)
    {
        int rows = map.GetLength(0);
        int columns = map.GetLength(1);
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
            }
            else
            {
                // Actual collapse algorithim
            }
        }
    }
}
