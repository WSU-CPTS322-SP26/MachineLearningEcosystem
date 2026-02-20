using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.VirtualTexturing;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private int xDim = 5;
    [SerializeField] private int yDim = 5;
    [SerializeField] private int terrainSize = 64;
    [SerializeField] private List<MapTerrain> terrainOptions;
    private WFCCell[,] cells;
    private MapTerrain[,] map;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        cells = new WFCCell[xDim,yDim];
        map = new MapTerrain[xDim,yDim];
        InitializeCells();
        ProceduralGeneration.CollapseWaveFunction(cells);
        DisplayMap();
    }

    private void DisplayMap()
    {
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (map[i,j] == null)
                {
                    if (cells[i,j].GetTerrain() == null)
                    {
                        Debug.Log("Error: Cell at " + cells[i,j].placement + " has no terrain assigned");
                    }
                    map[i,j] = Instantiate(cells[i,j].GetTerrain(),
                        new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0),
                        Quaternion.identity, gameObject.transform);
                    map[i,j].gameObject.SetActive(true);
                }
            }
        }
    }
    private void InitializeCells()
    {
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i,j] = new WFCCell(i,j);
                cells[i,j].possibleTiles = new List<MapTerrain>(terrainOptions);
            }
        }
    }
    public WFCCell[,] GetCells()
    {
        return cells;
    }
}
