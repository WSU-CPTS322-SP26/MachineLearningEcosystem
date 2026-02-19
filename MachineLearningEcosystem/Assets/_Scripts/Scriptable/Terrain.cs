using UnityEngine;

[CreateAssetMenu(fileName = "Terrain", menuName = "Scriptable Objects/Terrain")]
public class Terrain : ScriptableObject
{
    [SerializeField] private string terrain_type;
    [SerializeField] private Sprite texture;
}
