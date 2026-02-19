using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class MapTerrain : MonoBehaviour
{
    [SerializeField] private string terrain_type;
    [SerializeField] private float weight;
    [SerializeField] private SpriteRenderer sprite;

    public MapTerrain()
    {
        terrain_type = "empty";
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
}
