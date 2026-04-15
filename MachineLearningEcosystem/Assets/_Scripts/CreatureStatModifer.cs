using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStatModifer
{
    [SerializeField] private float mutationPercentage;
    private static Dictionary<string, Tuple<float, float>> maxMins = new Dictionary<string, Tuple<float, float>>
        {
            ["Health"] = new Tuple<float, float>(25f, 500f),
            ["Damage"] = new Tuple<float, float>(1f, 150f),
            ["Range"] = new Tuple<float, float>(0.2f, 4f),
            ["ViewDistance"] = new Tuple<float, float>(10f, 100f),
            ["ViewAngle"] = new Tuple<float, float>(20f, 340f),
            ["Speed"] = new Tuple<float, float>(1f, 20f),
            ["Thirst"] = new Tuple<float, float>(40f, 1000f),
            ["Hunger"] = new Tuple<float, float>(40f, 1000f)
        };

    public static Dictionary<string, float> ModifyStats(Dictionary<string, float> seedStats, float mutationPercent, int mutationCount = 3)
    {
        Dictionary<string, float> newStats = new Dictionary<string, float>(seedStats);

        List<string> keys = new List<string>(seedStats.Keys);

        for (int i = 0; i < mutationCount; i++)
        {
            // choosing stats to modify
            string statA = keys[UnityEngine.Random.Range(0, keys.Count)];
            string statB = keys[UnityEngine.Random.Range(0, keys.Count)];

            //if choosen stats are the same :( try again
            // also prevent scaling of current stats, they are set to max on start
            if (statA == statB || statA.StartsWith("curr") || statB.StartsWith("curr"))
            {
                i--;
                continue;
            }

            // Mutation amount based on % of current value
            float deltaA = seedStats[statA] * mutationPercent * UnityEngine.Random.Range(0.5f, 1.5f);;
            float deltaB = seedStats[statB] * mutationPercent * UnityEngine.Random.Range(0.5f, 1.5f);

            // Randomly decide which stat goes up, so each benefit corresponds to a negative
            if (UnityEngine.Random.value > 0.5f)
            {
                newStats[statA] += deltaA;
                newStats[statB] -= deltaB;
            }
            else
            {
                newStats[statA] -= deltaA;
                newStats[statB] += deltaB;
            }

            // Get min and max values for scaling
            Tuple<float, float> maxA = GetMaxValue(statA);
            Tuple<float, float> maxB = GetMaxValue(statB);

            // Clamp to prevent going beyond true min and true max values
            if (maxA == null)
            {
                newStats[statA] = Mathf.Clamp(newStats[statA], maxA.Item1, seedStats[statA] * 2);
            }
            else
            {
                newStats[statA] = Mathf.Clamp(newStats[statA], maxA.Item1, maxA.Item2);
            }
            
            if (maxB == null)
            {
                newStats[statB] = Mathf.Clamp(newStats[statB], maxB.Item1, seedStats[statB] * 2);
            }
            else
            {
                newStats[statB] = Mathf.Clamp(newStats[statB], maxB.Item1, maxB.Item2);
            }
        }

        return newStats;
    }

    private static Tuple<float, float> GetMaxValue(string stat)
    {
        // get name of max stat
        if (maxMins.ContainsKey(stat))
            return maxMins[stat];

        // Fallback: use null, registered as no value
        return null;
    }
}
