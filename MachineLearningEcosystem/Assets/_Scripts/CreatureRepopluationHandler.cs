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

    [SerializeField] private GameObject carnivorCreature; // Prefabs
    [SerializeField] private GameObject herbivoreCreature;

    [SerializeField] public int carnivorCount = 20;
    [SerializeField] public int herbivoreCount = 20;

    [Header("Auto Respawn Settings")]
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private float respawnIntervalSec = 120f; // respawn every 2 minutes

    private static Dictionary<string, float> seedStats; // Used to mutate stats
    private List<GameObject> carnivorList = new List<GameObject>();
    private List<GameObject> herbivoreList = new List<GameObject>();

    private float respawnTimer = 0f;
    private bool simStarted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            seedStats = CreatureStatistics.GetBaseStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        //Debug.Log("creature repo here");
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

    void Update()
    {
        if (!simStarted || !autoRespawn) return;

        respawnTimer += Time.deltaTime;

        if (respawnTimer >= respawnIntervalSec)
        {
            respawnTimer = 0f;
            RespawnAll();
        }
    }

    public void RespawnAll()
    {
        Debug.Log("[Respawn] Triggering learning update before respawn...");

        // Force a learning update before wiping creatures
        // so any buffered experiences are used, not discarded
        SimulationManager.instance?.ForceLearningUpdate();

        Debug.Log("[Respawn] Respawning all creatures...");
        DeactivateAll();
        SpawnCreatures(carnivorCount, herbivoreCount);
    }

    private void DeactivateAll()
    {
        foreach (GameObject c in carnivorList)
        {
            if (c == null) continue;

            // Flag as dead so CreatureMovement stops its update loop
            CreatureMovement cm = c.GetComponent<CreatureMovement>();
            if (cm != null) cm.ForceDeactivate();

            c.SetActive(false);
        }

        foreach (GameObject c in herbivoreList)
        {
            if (c == null) continue;

            CreatureMovement cm = c.GetComponent<CreatureMovement>();
            if (cm != null) cm.ForceDeactivate();

            c.SetActive(false);
        }
    }


    public static void SpawnCreatures()
    {
        instance.SpawnCreatures(instance.carnivorCount, instance.herbivoreCount);
    }

    private void SpawnCreatures(int carnivoreCount, int herbivoreCount)
    {
        Debug.Log("Spawning creatures");
        MapTerrain[,] map = MapManager.instance.GetMap();
        int xDim = MapManager.GetXDim();
        int yDim = MapManager.GetYDim();

        // Spawn carnivores
        foreach (GameObject creature in carnivorList)
        {
            creature.SetActive(true);
            Vector3 pos = GetValidSpawnPosition(map, xDim, yDim);
            creature.transform.position = pos;

            // Reset stats and register with the shared brain
            CreatureMovement cm = creature.GetComponent<CreatureMovement>();
            if (cm != null)
            {
                cm.ResetCreature();
                SimulationManager.instance?.RegisterCreature(cm);
                cm.SetStats(CreatureStatModifer.ModifyStats(seedStats, Random.Range(0.1f, 0.3f)));
            }
        }

        // Spawn herbivores
        foreach (GameObject creature in herbivoreList)
        {
            creature.SetActive(true);
            Vector3 pos = GetValidSpawnPosition(map, xDim, yDim);
            creature.transform.position = pos;

            CreatureMovement cm = creature.GetComponent<CreatureMovement>();
            if (cm != null)
            {
                cm.ResetCreature();
                SimulationManager.instance?.RegisterCreature(cm);
                cm.SetStats(CreatureStatModifer.ModifyStats(seedStats, Random.Range(0.1f, 0.3f)));
            }
        }
    }

    // --- Extracted helper: find a valid non-water spawn tile ---
    private Vector3 GetValidSpawnPosition(MapTerrain[,] map, int xDim, int yDim)
    {
        int x = Random.Range(0, xDim);
        int y = Random.Range(0, yDim);
        while (map[x, y].GetTerrainData().GetTerrainType() == "water")
        {
            x = Random.Range(0, xDim);
            y = Random.Range(0, yDim);
        }
        return map[x, y].transform.position;
    }


    private void OnPlayButtonClicked()
    {
        //Debug.Log("start button pressed");
        if (ProceduralGeneration.IsGenerating() || !MapManager.instance.IsMapGenerated())
        {
            return;
        }
        // gameObject.SetActive(false);

        SpawnCreatures(carnivorCount, herbivoreCount);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
    }
}
