using System.Collections.Generic;
// using System.Data;
// using System.Runtime.CompilerServices;
// using UnityEditor.TerrainTools;
// using UnityEditor.U2D.Aseprite;
using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Rendering.UI;
// using UnityEngine.Rendering.VirtualTexturing;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private static int xDim = 80;
    [SerializeField] private static int yDim = 50;
    [SerializeField] private static float terrainSize = 3;
    [SerializeField] public List<TerrainData> terrainOptions;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private TerrainData emptyData;
    private WFCCell[,] cells;
    private MapTerrain[,] map;
    private ObjectPool<MapTerrain> terrainPool;

    private void Awake()
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

    private void Start()
    {
        terrainPool = new(terrainPrefab.GetComponent<MapTerrain>(), 4000, gameObject.transform);
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
                    MapTerrain newTerrain = terrainPool.Get();
                    map[i,j] = newTerrain;
                    newTerrain.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                    map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                    map[i,j].SetTerrainData(cells[i,j].GetTerrain());
                }
                else
                {
                    map[i,j].gameObject.SetActive(true);
                    map[i,j].SetTerrainData(cells[i,j].GetTerrain());
                    map[i,j].gameObject.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                    map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
                }
            }
        }
    }
    public void DisplayBlankMap()
    {
        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i,j].gameObject.SetActive(true);
                map[i,j].SetTerrainData(emptyData);
                map[i,j].gameObject.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
                map[i,j].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
            }
        }
    }
    public void DisplayCell(WFCCell cell)
    {
        Vector2Int pos = cell.placement;
        if (map[pos.x, pos.y] == null)
        {
            MapTerrain newTerrain = terrainPool.Get();
            map[pos.x, pos.y].gameObject.SetActive(true);
            map[pos.x, pos.y] = newTerrain;
            newTerrain.transform.position = new Vector3(pos.x * terrainSize - (xDim / 2f * terrainSize), pos.y * terrainSize - (yDim / 2f * terrainSize), 0);
            map[pos.x, pos.y].gameObject.transform.localScale = new Vector3(terrainSize, terrainSize, 1);
        }
        map[pos.x, pos.y].gameObject.SetActive(true);
        map[pos.x, pos.y].SetTerrainData(cell.GetTerrain());
    }
    private void InitializeCells()
    {
        if (map != null)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    terrainPool.ReturnToPool(map[i,j].ResetMapTerrain());
                }
            }
        }
        cells = new WFCCell[xDim,yDim];
        map = new MapTerrain[xDim,yDim];
        for (int i = 0; i < cells.GetLength(0); i++) {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                MapTerrain newTerrain = terrainPool.Get();
                map[i,j] = newTerrain;
                newTerrain.transform.position = new Vector3(i * terrainSize - (xDim / 2f * terrainSize), j * terrainSize - (yDim / 2f * terrainSize), 0);
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
        PlantManager.instance?.ClearAllPlants();
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

    public static float GetTerrainSize()
    {
        return terrainSize;
    }
    public void SetTerrainSize(float x)
    {
        terrainSize = x;
    }

    public Vector2[] GetCorners()
    {
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(-xDim / 2f * terrainSize, -yDim / 2f * terrainSize); // Bottom left
        corners[1] = new Vector2(xDim / 2f * terrainSize, -yDim / 2f * terrainSize); // Bottom right
        corners[2] = new Vector2(xDim / 2f * terrainSize, yDim / 2f * terrainSize); // Top right
        corners[3] = new Vector2(-xDim / 2f * terrainSize, yDim / 2f * terrainSize); // Top left
        return corners;
    }

    //getter for map array to get accessed for PlantManager
    public MapTerrain[,] GetMap()
    {
        return map;
    }

    public bool IsMapGenerated()
    {
        if (map == null || cells == null)
        {
            return false;
        }
        return true;
    }
}