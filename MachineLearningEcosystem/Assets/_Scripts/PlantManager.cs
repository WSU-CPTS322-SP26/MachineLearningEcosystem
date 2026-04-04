using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private int maxPlants = 50; //MAX AMOUNT OF PLANTS
    [SerializeField] private float growIntervalMin = 1f; //MINIMUM WAIT SPEED 
    [SerializeField] private float growIntervalMax = 10f; //MAX WAIT SPEED
    private List<MapTerrain> grassTiles;
    private List<GameObject> plantObjects = new();
    private ObjectPool<PlantInstance> plantPool;
    private bool isGrowing = false;

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

    private void Start()
    {
        plantPool = new(plantPrefab.GetComponent<PlantInstance>(), 150, instance.gameObject.transform);
    }

    //run after map generates
    public void Initialize()
    {
        //Debug.Log("PlantManager initializing...");


        //Map from MapManager
        MapTerrain[,] map = MapManager.instance.GetMap();

        //Initialize list field
        grassTiles = new List<MapTerrain>();

        
        //cell amount.
        int xDim = MapManager.GetXDim();
        int yDim = MapManager.GetYDim();
        //Debug.Log("Mapsize: " + xDim + " x " + yDim);


        //find all grass tiles
        foreach (MapTerrain tile in map) {
            if (tile.GetTerrainData() != null && tile.GetTerrainData().GetTerrainType() == "grass")
            {
                grassTiles.Add(tile);
            }
        }

        maxPlants = (int)(grassTiles.Count * .05f); // ~ 1/20 grass tiles has a plant food
        GrowInitialPlants();
        // Debug.Log("Found " + grassTiles.Count + " grass tiles!"); 
    }

    // Instantly grow max plants
    private void GrowInitialPlants()
    {
        List<MapTerrain> availableTiles = new List<MapTerrain>(grassTiles);

        while (availableTiles.Count > 0 && plantObjects.Count < maxPlants)
        {
            int index = UnityEngine.Random.Range(0, availableTiles.Count);
            MapTerrain tile = availableTiles[index];
            availableTiles.RemoveAt(index);

            Vector3 spawnPos = tile.transform.position + (Vector3.up * Random.Range(-2f, 2f)) + (Vector3.right * Random.Range(-2f, 2f));
            //plantObjects.Add(Instantiate(plantPrefab, spawnPos, Quaternion.identity));
            PlantInstance newPlant = plantPool.Get();
            plantObjects.Add(newPlant.gameObject);
            newPlant.transform.position = spawnPos;
        }
    }

    //grow plants through interval and have a maximum amount of plants limit
    private IEnumerator GrowPlants()
    {
        List<MapTerrain> availableTiles = new List<MapTerrain>(grassTiles);

        while (availableTiles.Count > 0 && plantObjects.Count < maxPlants)
        {
            int index = UnityEngine.Random.Range(0, availableTiles.Count);
            MapTerrain tile = availableTiles[index];
            availableTiles.RemoveAt(index);

            Vector3 spawnPos = tile.transform.position + (Vector3.up * Random.Range(-2f, 2f)) + (Vector3.right * Random.Range(-2f, 2f));
            // plantObjects.Add(Instantiate(plantPrefab, spawnPos, Quaternion.identity));
            PlantInstance newPlant = plantPool.Get();
            plantObjects.Add(newPlant.gameObject);
            newPlant.transform.position = spawnPos;

            float waitTime = UnityEngine.Random.Range(growIntervalMin, growIntervalMax);
            yield return new WaitForSeconds(waitTime);
        }
        isGrowing = false;
        //Debug.Log("Finsihed growing plants!");
    }

    public void ClearPlant(GameObject plant)
    {
        if (plantObjects.Contains(plant))
        {
            plantObjects.Remove(plant);
            plantPool.ReturnToPool(plant.GetComponent<PlantInstance>());
        }
        if (!isGrowing)
        {
            StartCoroutine(GrowPlants());
            isGrowing = true;
        }
    }

    public void ClearAllPlants()
    {
        foreach (GameObject plant in plantObjects)
        {
            plantPool.ReturnToPool(plant.GetComponent<PlantInstance>());
        }
        plantObjects.Clear();
    }
}
