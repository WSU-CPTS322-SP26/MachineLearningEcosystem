using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class MapTerrain : MonoBehaviour
{
    [SerializeField] private string terrainType;
    [SerializeField] private List<string> possibleNs; // possible neighbors
    [SerializeField] private float weight;
    [SerializeField] private SpriteRenderer sprite;

    public MapTerrain()
    {
        terrainType = "empty";
        weight = 0f;
    }
    public float GetWeight()
    {
        return weight;
    }
    public void SetWeight(float newWeight)
    {
        weight = newWeight;
    }
    public List<string> GetPossibleNS()
    {
        return possibleNs;
    }
    public string GetTerrainType()
    {
        return terrainType;
    }
}
