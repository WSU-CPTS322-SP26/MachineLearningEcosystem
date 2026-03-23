using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    private List<MapTerrain> grassTiles;

    [SerializeField] private GameObject plantPrefab;

    [SerializeField] private int maxPlants = 50; //MAX AMOUNT OF PLANTS

    [SerializeField] private float growIntervalMin = 1f; //MINIMUM WAIT SPEED 

    [SerializeField] private float growIntervalMax = 4f; //MAX WAIT SPEED

    private int currentPlantCount = 0;

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
    }

    //run after map generates
    public void Initialize()
    {
        Debug.Log("PlantManager initializing..."); //can delete later


        //Map from MapManager
        MapTerrain[,] map = MapManager.instance.GetMap();

        //Initialize list field
        grassTiles = new List<MapTerrain>();

        
        //cell amount.
        int xDim = MapManager.GetXDim();
        int yDim = MapManager.GetYDim();
        Debug.Log("Mapsize: " + xDim + " x " + yDim); //can erase after


        //find all grass tiles
        for (int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                MapTerrain tile = map[x, y];
                if (tile.GetTerrainData() != null && tile.GetTerrainData().GetTerrainType() == "grass")
                {
                    grassTiles.Add(tile);
                }
            }
        }

        Debug.Log("Found " + grassTiles.Count + " grass tiles!"); //can delete later


        StartCoroutine(GrowPlants());
 
    }


    //grow plants through interval and have a maximum amount of plants limit
    private IEnumerator GrowPlants()
    {
        List<MapTerrain> availableTiles = new List<MapTerrain>(grassTiles);

        while (availableTiles.Count > 0 && currentPlantCount < maxPlants)
        {
            int index = UnityEngine.Random.Range(0, availableTiles.Count);
            MapTerrain tile = availableTiles[index];
            availableTiles.RemoveAt(index);

            Vector3 spawnPos = tile.transform.position;
            Instantiate(plantPrefab, spawnPos, Quaternion.identity);

            currentPlantCount++;

            float waitTime = UnityEngine.Random.Range(growIntervalMin, growIntervalMax);
            yield return new WaitForSeconds(waitTime);
        }

        Debug.Log("Finsihed growing plants!");
    }

 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
