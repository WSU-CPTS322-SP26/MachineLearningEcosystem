using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;



//class handles the popluation of creatures on the generated map. 
//does not spawn creatures on water tiles and creatures must spawn inside the bounds of the map.
public class CreatureRepopluationHandler : MonoBehaviour
{

    public static CreatureRepopluationHandler instance;

    [SerializeField] private Button playButton;

    [SerializeField] private GameObject carnivorCreature;
    [SerializeField] private GameObject herbivoreCreature;

    [SerializeField] private int carnivorCount;
    [SerializeField] private int herbivoreCount;


    private List<GameObject> carnivorList = new List<GameObject>();
    private List<GameObject> herbivoreList = new List<GameObject>();



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


    void Start()
    {


        Debug.Log("creature repo here");
        playButton.onClick.AddListener(OnPlayButtonClicked);

        //instantiate all carnivors and herbivores to use object pooling for slight optimization 
        herbivoreList.Clear();
        carnivorList.Clear();
        
        for (int i = 0; i < carnivorCount; i++)
        {
            carnivorList.Add(Instantiate(carnivorCreature));
            carnivorList[i].SetActive(false);
        }
        for (int i = 0; i < herbivoreCount; i++)
        {
            herbivoreList.Add(Instantiate(herbivoreCreature));
            herbivoreList[i].SetActive(false);
        }
    }

    
    public static void SpawnCreatures()
    {
        instance.SpawnCreatures(instance.carnivorCount, instance.herbivoreCount);
    }
    
    private void SpawnCreatures(int carnivoreCount, int herbivoreCount)
    {
        Debug.Log("spawning creatures");

        MapTerrain[,] map = MapManager.instance.GetMap();

        //map diminsions
        int xDim = MapManager.GetXDim();
        int yDim = MapManager.GetYDim();

        

        

        //spawning for carnivors
        foreach (GameObject creature in carnivorList)
        {
            creature.SetActive(true);
            Vector2 spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            while (map[(int)spawnPoint.x, (int)spawnPoint.y].GetTerrainData().GetTerrainType() == "water")
            {
                spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            }
            creature.transform.position = spawnPoint;
        }

        //spawning for herbavoris
        foreach (GameObject creature in herbivoreList)
        {
            creature.SetActive(true);
            Vector2 spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            while (map[(int)spawnPoint.x, (int)spawnPoint.y].GetTerrainData().GetTerrainType() == "water")
            {
                spawnPoint = new Vector2(Random.Range(0, xDim), Random.Range(0, yDim));
            }
            creature.transform.position = spawnPoint;
        }

    }


    private void OnPlayButtonClicked()
    {
        Debug.Log("start button pressed");
        if (ProceduralGeneration.IsGenerating())
        {
            return;
        }
        gameObject.SetActive(false);

        SpawnCreatures(carnivorCount, herbivoreCount);
    }
}
