using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int xDim = 5;
    [SerializeField] private int yDim = 5;
    [SerializeField] private int terrainSize = 64;
    [SerializeField] private List<MapTerrain> terrainOptions;
    private WFCCell[,] cells;
    private MapTerrain[,] map;

    private void Start()
    {
        cells = new WFCCell[xDim,yDim];
        map = new MapTerrain[xDim,yDim];
        InitializeMap();
        DisplayMap();
    }

    private void DisplayMap()
    {
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                //cells[i,j].position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                map[i,j] = Instantiate(cells[i,j].GetTerrain(),
                    new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0),
                    Quaternion.identity, gameObject.transform);
                map[i,j].gameObject.SetActive(true);
            }
        }
    }
    private void InitializeMap()
    {
        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                
            }
        }
    }
}
