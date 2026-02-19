using System.Collections;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    // Using the procedural generation model of wave function collapse, this program is
    // designed to generate an x by y sized tilemap of different terrains based on
    // constraints for terrain placement
    [SerializeField] private int xDimension = 5;
    [SerializeField] private int yDimension = 5;
    private int iter = 0;
    private int maxIter;

    void Start()
    {
        maxIter = xDimension * yDimension;
    }

    void CollapseWaveFunction()
    {
        bool fullyCollapsed = false;
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
