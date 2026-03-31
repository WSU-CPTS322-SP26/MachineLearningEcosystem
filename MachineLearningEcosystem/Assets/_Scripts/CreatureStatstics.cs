using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System;

//HAVE A FUNCTION WHERE IT RANDOMIZES THE CREATURE STATISTICS WITHOUT 
//EXCEEDING THE MAX AND MIN VALUES OF THE VARIABLES
public class CreatureStatstics : MonoBehaviour
{
    private Dictionary<string, float> stats = new();

    [SerializeField] private string _creatureName;
    private float statTimer = 0f;
    public event Action DeathSignal;

    private void Awake()
    {
        stats.Add("Health", 100);
        stats.Add("maxHealth", 100);

        stats.Add("ViewDistance", 20);
        stats.Add("ViewAngle", 70);

        stats.Add("Speed", 5);
        stats.Add("MaxSpeed", 20);

        stats.Add("Thirst", 200);
        stats.Add("ThirstLossRate", 1);
        stats.Add("maxThirst", 100);

        stats.Add("Hunger", 100);
        stats.Add("HungerLossRate", 1);
        stats.Add("maxHunger", 100);

        // stats.Add("Size", 2);
    }

    //setters and getters
    public string CreatureName
    {
        get { return _creatureName; }
        set { _creatureName = value; }
    }

    public float getStat(string name)
    {
        return stats[name];
    }


    public float Health
    {
        get { return stats["Health"]; }
        set {
            stats["Health"] = Mathf.Clamp(value, 0, MaxHealth);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
    }
    public float MaxHealth
    {
        get { return stats["maxHealth"]; }
        set { stats["maxHealth"] = value; }
    }
    public float ViewDistance
    {
        get { return stats["ViewDistance"]; }
        set { stats["ViewDistance"] = value; }
    }
    public float ViewAngle
    {
        get { return stats["ViewAngle"]; }
        set { stats["ViewAngle"] = value; }
    }
    public float Speed
    {
        get { return stats["Speed"]; }
        set { stats["Speed"] = value; }
    }
    public float Thirst
    {
        get { return stats["Thirst"]; }
        set {
            stats["Thirst"] = Mathf.Clamp(value, 0, MaxThirst);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
    }
    public float ThirstLossRate
    {
        get { return stats["ThirstLossRate"]; }
        set { stats["ThirstLossRate"] = value; }
    }
    public float MaxThirst
    {
        get { return stats["maxThirst"]; }
        set { stats["maxThirst"] = value; }
    }

    public float Hunger
    {
        get { return stats["Hunger"]; }
        set {
            stats["Hunger"] = Mathf.Clamp(value, 0, MaxHunger);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
    }
    public float HungerLossRate
    {
        get { return stats["HungerLossRate"]; }
        set { stats["HungerLossRate"] = value; }
    }
    public float MaxHunger
    {
        get { return stats["maxHunger"]; }
        set { stats["maxHunger"] = value; }
    }
  
    // public float Size
    // {
    //     get { return stats["Size"]; }
    //     set { stats["Size"] = value; }
    // }

    public void RandomizeStats()
    {
        // Implement randomization logic here, ensuring that values do not exceed max and min limits
    }

    private void Update()
    {
        statTimer += Time.deltaTime;
        if (statTimer > 1f)
        {
            // Every 1 second, hunger and thirst tick down by their respective rates
            // Other stats that change over time can be added to this check
            Hunger -= HungerLossRate;
            Thirst -= ThirstLossRate;
            statTimer = 0f;
        }
    }
}
