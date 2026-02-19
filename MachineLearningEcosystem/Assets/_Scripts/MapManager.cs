using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int xDim = 5;
    [SerializeField] private int yDim = 5;
    [SerializeField] private int terrainSize = 64;
    [SerializeField] private Terrain[] terrainOptions;
    private Terrain[,] map;

    private void Start()
    {
        map = new Terrain[xDim,yDim];
        //ProceduralGeneration.CollapseWaveFunction(map, terrainOptions);
        ProceduralGeneration.RandomizeTiles(map, terrainOptions);
        DisplayMap();
    }

    private void DisplayMap()
    {
        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i,j].transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                map[i,j].gameObject.SetActive(true);
            }
        }
    }
}
