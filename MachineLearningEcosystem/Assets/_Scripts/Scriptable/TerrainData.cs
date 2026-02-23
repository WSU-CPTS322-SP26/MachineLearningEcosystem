using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Scriptable Objects/TerrainData")]
public class TerrainData : ScriptableObject
{
    [SerializeField] private string terrainType;
    [SerializeField] private List<string> possibleNs; // possible neighbors
    [SerializeField] private List<float> possibleNsWeights; // possible neighbors weights
    [SerializeField] private Sprite sprite;
    private float weight = 1f;
    public TerrainData(string type, List<string> possibleNeighbors, List<float> possibleNeighborsWeights, Sprite s)
    {
        terrainType = type;
        possibleNs = possibleNeighbors;
        possibleNsWeights = possibleNeighborsWeights;
        sprite = s;
        weight = 1f;
    }
    public void Copy(TerrainData data)
    {
        terrainType = data.GetTerrainType();
        possibleNs = data.GetPossibleNS().Keys.ToList();
        possibleNsWeights = new List<float>();
        foreach (string key in possibleNs)
        {
            possibleNsWeights.Add(data.GetPossibleNS()[key]);
        }
        sprite = data.GetSprite();
        weight = 1f;
    }
    public string GetTerrainType()
    {
        return terrainType;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
    public Dictionary<string, float> GetPossibleNS()
    {
        Dictionary<string, float> dict = new Dictionary<string, float>();
        for (int i = 0; i < possibleNs.Count; i++)
        {
            dict.Add(possibleNs[i], possibleNsWeights[i]);
        }
        return dict;
    }
    public float GetWeight()
    {
        return weight;
    }
    public void SetWeight(float w)
    {
        weight = w;
    }
}
