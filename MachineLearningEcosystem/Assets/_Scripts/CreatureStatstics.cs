using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System;

public class CreatureStatstics : MonoBehaviour
{
    private Dictionary<string, float> stats = new();

    [SerializeField] private string _creatureName;
    private float statTimer = 0f;
    public event Action DeathSignal;

    private void Awake()
    {
        stats.Add("Health", 100);
        stats.Add("currHealth", 100);

        stats.Add("ViewDistance", 20);
        stats.Add("ViewAngle", 70);

        stats.Add("Speed", 5);

        stats.Add("Thirst", 200);
        stats.Add("currThirst", 200);

        stats.Add("Hunger", 100);
        stats.Add("currHunger", 100);

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
        set { stats["Health"] = value; }
    }
    public float CurrHealth
    {
        get { return stats["currHealth"]; }
        set {
            stats["currHealth"] = Mathf.Clamp(value, 0, Health);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
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
        set { stats["Thirst"] = value; }
        
    }
    // public float ThirstLossRate
    // {
    //     get { return stats["ThirstLossRate"]; }
    //     set { stats["ThirstLossRate"] = value; }
    // }
    public float CurrThirst
    {
        get { return stats["currThirst"]; }
        set {
            stats["currThirst"] = Mathf.Clamp(value, 0, Thirst);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
    }

    public float Hunger
    {
        get { return stats["Hunger"]; }
        set { stats["Hunger"] = value; }
    }
    // public float HungerLossRate
    // {
    //     get { return stats["HungerLossRate"]; }
    //     set { stats["HungerLossRate"] = value; }
    // }
    public float CurrHunger
    {
        get { return stats["currHunger"]; }
        set {
            stats["currHunger"] = Mathf.Clamp(value, 0, Hunger);
            if (value <= 0)
            {
                DeathSignal?.Invoke();
            }
        }
    }
  
    // public float Size
    // {
    //     get { return stats["Size"]; }
    //     set { stats["Size"] = value; }
    // }

    private void Update()
    {
        statTimer += Time.deltaTime;
        if (statTimer > 1f)
        {
            // Every 1 second, hunger and thirst tick down by 1
            // Other stats that change over time can be added to this check
            Hunger -= 1f;
            Thirst -= 1f;
            statTimer = 0f;
        }
    }
}
