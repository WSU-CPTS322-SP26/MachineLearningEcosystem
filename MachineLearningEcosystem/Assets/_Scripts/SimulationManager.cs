using UnityEngine;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance { get; private set; }



    [Header("Settings")]
    private int creatureCount;
    [SerializeField] private int learnIntervalSteps = 2048;
    [SerializeField] private bool autoSave = true;
    [SerializeField] private int autoSaveInterval = 10000;

    // The ONE shared brain
    private SharedBrain sharedBrain;
    private List<CreatureMovement> creatures = new();
    private int stepCount = 0;
    private int nextId = 0;

    // Network architecture � must match CreatureMovement constants
    private static readonly int[] NetworkShape ={ CreatureMovement.STATE_SIZE, 128, 128, CreatureMovement.ACTION_SIZE };



    void Awake()
    {
        // ADD singleton setup
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [System.Obsolete] // TODO: Remove and fix the findobjectsoftype call
    void Start()
    {
        //creatureCount = CreatureRepopluationHandler.carinavorCount

        // Create the single shared brain
        sharedBrain = new SharedBrain(NetworkShape);

        // Find initial creatures
        CreatureMovement[] existing = FindObjectsOfType<CreatureMovement>();
        foreach (CreatureMovement c in existing)
            RegisterCreature(c);
    }


    // Used by CreatureRepopulationHandler just before a full respawn
    // so buffered experiences aren't thrown away
    public void ForceLearningUpdate()
    {
        sharedBrain.Update();
        Debug.Log($"[SimulationManager] Forced learning update at step {stepCount}");
    }

    void Update()
    {
        stepCount++;

        // Trigger the shared brain update every N steps
        if (stepCount % learnIntervalSteps == 0)
            sharedBrain.Update();

        // Auto save
        if (autoSave && stepCount % autoSaveInterval == 0)
            SaveBrain();

        // Clean up destroyed creatures
        creatures.RemoveAll(c => c == null);
    }

    // Call this whenever a new creature is spawned
    public void RegisterCreature(CreatureMovement creature)
    {
        creature.Initialize(nextId++, sharedBrain);
        creatures.Add(creature);
    }

    public void SaveBrain()
    {
        NeuralNetworkSaver.Save(sharedBrain.GetActor(), "shared_actor");
        NeuralNetworkSaver.Save(sharedBrain.GetCritic(), "shared_critic");
        Debug.Log($"Shared brain saved at step {stepCount}");
    }

    public void LoadBrain()
    {
        NeuralNetworkSaver.Load(sharedBrain.GetActor(), "shared_actor");
        NeuralNetworkSaver.Load(sharedBrain.GetCritic(), "shared_critic");
        Debug.Log("Shared brain loaded");
    }
}