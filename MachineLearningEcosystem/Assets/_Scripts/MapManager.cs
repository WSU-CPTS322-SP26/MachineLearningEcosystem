using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEditor.TerrainTools;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.VirtualTexturing;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private static int xDim = 80;
    [SerializeField] private static int yDim = 50;
    [SerializeField] private float terrainSize = 64;
    [SerializeField] private List<TerrainData> terrainOptions;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private TerrainData emptyData;
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
        ProceduralGeneration.ResetGeneration();
    }
    public void DisplayMap()
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
                    map[i,j] = Instantiate(terrainPrefab,
                        new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0),
                        Quaternion.identity, gameObject.transform).GetComponent<MapTerrain>();
                    map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                    map[i,j].SetTerrainData(cells[i,j].GetTerrain());
                    map[i,j].gameObject.SetActive(true);
                }
                else
                {
                    map[i,j].SetTerrainData(cells[i,j].GetTerrain());
                    map[i,j].gameObject.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                    map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                    map[i,j].gameObject.SetActive(true);
                }
            }
        }
    }
    public void DisplayBlankMap()
    {
        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i,j].SetTerrainData(emptyData);
                map[i,j].gameObject.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                map[i,j].gameObject.SetActive(true);
            }
        }
    }
    public void DisplayCell(WFCCell cell)
    {
        Vector2Int pos = cell.placement;
        if (map[pos.x, pos.y] == null)
        {
            map[pos.x, pos.y] = Instantiate(terrainPrefab,
                new Vector3(pos.x * terrainSize - (xDim / 2f * terrainSize), pos.y * terrainSize - (yDim / 2f * terrainSize), 0),
                Quaternion.identity, gameObject.transform).GetComponent<MapTerrain>();
            map[pos.x, pos.y].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
        }
        map[pos.x, pos.y].SetTerrainData(cell.GetTerrain());
        map[pos.x, pos.y].gameObject.SetActive(true);
    }
    private void InitializeCells()
    {
        if (map != null)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Destroy(map[i, j].gameObject);
                }
            }
        }
        cells = new WFCCell[xDim,yDim];
        map = new MapTerrain[xDim,yDim];
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                map[i, j] = Instantiate(terrainPrefab,
                    new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0),
                    Quaternion.identity, gameObject.transform).GetComponent<MapTerrain>();
                map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                cells[i, j] = new WFCCell(i, j)
                {
                    possibleTiles = new List<TerrainData>()
                };
                foreach (TerrainData t in terrainOptions)
                {
                    t.SetWeight(1f);
                    TerrainData temp = ScriptableObject.CreateInstance<TerrainData>();
                    temp.Copy(t);
                    cells[i,j].possibleTiles.Add(temp);
                }
            }
        }
    }
    public List<TerrainData> GetTerrainOptions()
    {
        return terrainOptions;
    }
    public void GenerateMap()
    {
        InitializeCells();
        DisplayBlankMap();
        StartCoroutine(ProceduralGeneration.CollapseWaveFunction(cells));
    }
    public static int GetXDim()
    {
        return xDim;
    }
    public static int GetYDim()
    {
        return yDim;
    }
    public void SetXDim(int x)
    {
        xDim = x;
    }
    public void SetYDim(int y)
    {
        yDim = y;
    }
}
