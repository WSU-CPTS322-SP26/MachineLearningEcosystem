using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System;

using Random = UnityEngine.Random;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using Unity.VisualScripting;

public class CreatureStatistics : MonoBehaviour
{
    private Dictionary<string, float> stats = new();

    private static List<string> possible_names = new();

    [SerializeField] private string _creatureName;
    [SerializeField] private bool _carnivorous;
    private float statTimer = 0f;
    public event Action DeathSignal;


    private void Awake()
    {
        if (possible_names.Count == 0)
        {
            Parse("names");
        }
        _creatureName = possible_names[Random.Range(0, possible_names.Count)];

        stats.AddRange(GetBaseStats());
    }

    public static Dictionary<string, float> GetBaseStats() {
        Dictionary<string, float> dict = new()
        {
            { "Health", 100 },
            { "currHealth", 100 },
            { "Damage", 25 },
            { "Range", 3 },
            { "ViewDistance", 20 },
            { "ViewAngle", 70 },
            { "Speed", 8 },
            { "Thirst", 100 },
            { "currThirst", 100 },
            { "Hunger", 150 },
            { "currHunger", 150 }
        };

        return dict;
    }


    private void Parse(string filename)
    {
        TextAsset asset = Resources.Load<TextAsset>(filename);
        string[] lines = asset.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            foreach (string name in columns)
            {
                possible_names.Add(name);
            }
        }
    }    

    //NEW
    private void OnMouseDown()
    {
        StatsBox.Instance.DisplayStats(this);
    }

    //setters and getters
    public string CreatureName
    {
        get { return _creatureName; }
        set { _creatureName = value; }
    }

    public bool IsCarnivore
    {
        get { return _carnivorous; }
        set { _carnivorous = value; }
    }

    public float getStat(string name)
    {
        return stats[name];
    }

    public float Health
    {
        get { return stats["Health"]; }
        set { stats["Health"] = (int)value; }
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

    public float Damage
    {
        get { return stats["Damage"]; }
        set { stats["Damage"] = (int)value; }
    }
    public float Range
    {
        get { return stats["Range"]; }
        set { stats["Range"] = value; }
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
        set { stats["Thirst"] = (int)value; }
        
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
        set { stats["Hunger"] = (int)value; }
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
            CurrHunger -= 1f;
            CurrThirst -= 1f;
            statTimer = 0f;
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = new(CurrHealth / Health, CurrHunger / Hunger, CurrThirst / Thirst);
        }
    }

    public void SetStats(Dictionary<string, float> newStats)
    {
        stats = newStats;
        CurrThirst = Thirst;
        CurrHealth = Health;
        CurrHunger = Hunger;
    }
}
