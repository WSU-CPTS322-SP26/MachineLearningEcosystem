using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class MapTerrain : MonoBehaviour
{
    [SerializeField] private Dictionary<string, float> possibleNs; // possible neighbors
    [SerializeField] private TerrainData terrainData;
    private float weight = 1f;
    private string terrainType;
    public MapTerrain()
    {
        terrainType = "empty";
        terrainData = null;
        weight = 1f;
    }
    private void Start()
    {
        if (terrainData == null)
        {
            Debug.Log("Error: No terrain data assigned to " + gameObject.name);
        }
        else
        {
            terrainType = terrainData.GetTerrainType();
            terrainData.GetPossibleNS(possibleNs);
        }
    }
    public float GetWeight()
    {
        return weight;
    }
    public Dictionary<string, float> GetPossibleNS()
    {
        return possibleNs;
    }
    public void SetWeight(float newWeight)
    {
        weight = newWeight;
    }
    public string GetTerrainType()
    {
        return terrainType;
    }
}
