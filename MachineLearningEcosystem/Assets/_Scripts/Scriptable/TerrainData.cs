using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Scriptable Objects/TerrainData")]
public class TerrainData : ScriptableObject
{
    [SerializeField] private string terrainType;
    [SerializeField] private List<string> possibleNs; // possible neighbors
    [SerializeField] private List<int> possibleNsWeights; // possible neighbors weights

    public string GetTerrainType()
    {
        return terrainType;
    }
    public List<Tuple<string, int>> GetPossibleNS()
    {
        List<Tuple<string, int>> options = new();
        for (int i = 0; i < possibleNs.Count; i++)
        {
            options.Add(new Tuple<string, int>(possibleNs[i], possibleNsWeights[i]));
        }
        return options;
    }
}
