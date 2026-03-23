using System.Collections.Generic;
using UnityEngine;

public class CreatureStatModifer : MonoBehaviour
{
    [SerializeField] private float mutationPercentage;


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
            if (statA == statB)
            {
                i--;
                continue;
            }

            //prevent scaling of max stats
            if (statA.StartsWith("max") == true || statB.StartsWith("max") == true)
            {
                i--;
                continue;
            }
                

            // Get max values for scaling
            float maxA = GetMaxValue(statA, seedStats);
            float maxB = GetMaxValue(statB, seedStats);

            // Mutation amount based on % of max
            float deltaA = maxA * mutationPercent * UnityEngine.Random.Range(0.5f, 1.5f);
            float deltaB = maxB * mutationPercent * UnityEngine.Random.Range(0.5f, 1.5f);

            // Randomly decide which stat goes up
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

            // Clamp to prevent negatives or insane values
            newStats[statA] = Mathf.Clamp(newStats[statA], 0f, maxA * 2f);
            newStats[statB] = Mathf.Clamp(newStats[statB], 0f, maxB * 2f);
        }

        return newStats;
    }




    private static float GetMaxValue(string stat, Dictionary<string, float> stats)
    {
        // get name of max stat
        string maxKey = "max" + stat;

        if (stats.ContainsKey(maxKey))
            return stats[maxKey];

        // Fallback: use its own value
        return stats[stat];
    }
}
