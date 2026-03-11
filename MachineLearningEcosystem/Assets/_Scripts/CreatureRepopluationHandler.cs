using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;



//class handles the popluation of creatures on the generated map. 
//does not spawn creatures on water tiles and creatures must spawn inside the bounds of the map.
public class CreatureRepopluationHandler : MonoBehaviour
{
    [SerializeField] private GameObject carnivorCreature;
    [SerializeField] private GameObject herbivoreCreature;

    [SerializeField] private int carnivorCount;
    [SerializeField] private int herbivoreCount;


    private List<GameObject> carnivorList = new List<GameObject>();
    private List<GameObject> herbivoreList = new List<GameObject>();


    //used to get size of generation field and type of tile creature is trying to be placed on
    private WFCCell[,] terrainCells = ProceduralGeneration.cells;
    private List<TerrainData> terrainCellOptions = MapManager.instance.terrainOptions;



    void Start()
    {
        //instantiate all carnivors and herbivores to use object pooling for slight optimization 
        herbivoreList.Clear();
        carnivorList.Clear();
        
        for (int i = 0; i < carnivorCount; i++)
        {
            carnivorList[i] = Instantiate(carnivorCreature);
        }
        for (int i = 0; i < herbivoreCount; i++)
        {
            herbivoreList[i] = Instantiate(herbivoreCreature);
        }
    }

    
    
    void SpawnCreatures(int carnivoreCount, int herbivoreCount)
    {
        int xDim = terrainCells.GetLength(0);
        int yDim = terrainCells.GetLength(1);
        
        //spawning for carnivors
        foreach (GameObject creature in carnivorList)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            while (terrainCells[(int)spawnPoint.x, (int)spawnPoint.y].GetTerrain() == terrainCellOptions[4])
            {
                spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            }
            creature.transform.position = spawnPoint;
        }

        //spawning for herbavoris
        foreach (GameObject creature in herbivoreList)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            while (terrainCells[(int)spawnPoint.x, (int)spawnPoint.y].GetTerrain() == terrainCellOptions[4])
            {
                spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            }
            creature.transform.position = spawnPoint;
        }

    }
}
