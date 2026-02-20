using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Scriptable Objects/TerrainData")]
public class TerrainData : ScriptableObject
{
    [SerializeField] private string terrainType;
    [SerializeField] private List<string> possibleNs; // possible neighbors
    [SerializeField] private List<float> possibleNsWeights; // possible neighbors weights

    public string GetTerrainType()
    {
        return terrainType;
    }
    public void GetPossibleNS(Dictionary<string, float> dict)
    {
        if (dict == null)
        {
            dict = new Dictionary<string, float>();
        }
        for (int i = 0; i < possibleNs.Count; i++)
        {
            dict.Add(possibleNs[i], possibleNsWeights[i]);
        }
    }
}
