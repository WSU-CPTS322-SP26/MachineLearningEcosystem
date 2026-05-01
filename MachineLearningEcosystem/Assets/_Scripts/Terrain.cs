using System;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class MapTerrain : MonoBehaviour
{
    [SerializeField] private TerrainData terrainData = null;
    public MapTerrain(TerrainData data)
    {
        terrainData = data;
    }
    public MapTerrain ResetMapTerrain()
    {
        terrainData = null;
        return this;
    }
    public void SetTerrainData(TerrainData data)
    {
        if (data != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = data.GetSprite();
            terrainData = data;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
            terrainData = null;
        }
    }
    public TerrainData GetTerrainData()
    {
        return terrainData;
    }
}
