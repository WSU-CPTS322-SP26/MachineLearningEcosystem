using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int learnIntervalSteps = 2048;
    [SerializeField] private bool autoSave = true;
    [SerializeField] private int autoSaveInterval = 10000;

    private List<CreatureMovement> creatures = new();
    private int stepCount = 0;

    void Start()
    {
        // Find all creatures already in the scene
        RefreshCreatureList();
    }

    void Update()
    {
        stepCount++;

        // Trigger PPO learning update every N steps
        if (stepCount % learnIntervalSteps == 0)
        {
            foreach (CreatureMovement creature in creatures)
            {
                if (creature != null && !creature.IsDead)
                    creature.Agent.Update();
            }
        }

        // Auto save brains periodically
        if (autoSave && stepCount % autoSaveInterval == 0)
            SaveAllBrains();

        // Clean up dead creatures and find any new ones
        RefreshCreatureList();
    }

    // Scan the scene for all CreatureMovement components
    public void RefreshCreatureList()
    {
        creatures.Clear();
        CreatureMovement[] found = FindObjectsOfType<CreatureMovement>();
        creatures.AddRange(found);
    }

    public void SaveAllBrains()
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] == null) continue;
            NeuralNetworkSaver.Save(creatures[i].Agent.GetActor(), $"creature_{i}_actor");
            NeuralNetworkSaver.Save(creatures[i].Agent.GetCritic(), $"creature_{i}_critic");
        }
        Debug.Log($"Saved {creatures.Count} brains at step {stepCount}");
    }

    public void LoadAllBrains()
    {
        RefreshCreatureList();
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] == null) continue;
            NeuralNetworkSaver.Load(creatures[i].Agent.GetActor(), $"creature_{i}_actor");
            NeuralNetworkSaver.Load(creatures[i].Agent.GetCritic(), $"creature_{i}_critic");
        }
    }
}

