using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class MapTerrain : MonoBehaviour
{
    [SerializeField] private Dictionary<string, float> possibleNs; // possible neighbors
    private TerrainData terrainData = null;
    public MapTerrain(TerrainData data)
    {
        terrainData = data;
        possibleNs = terrainData.GetPossibleNS();
    }
    public Dictionary<string, float> GetPossibleNS()
    {
        possibleNs = terrainData.GetPossibleNS();
        return possibleNs;
    }
    public void SetTerrainData(TerrainData data)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = data.GetSprite();
        terrainData = data;
    }
    public TerrainData GetTerrainData()
    {
        return terrainData;
    }
}
