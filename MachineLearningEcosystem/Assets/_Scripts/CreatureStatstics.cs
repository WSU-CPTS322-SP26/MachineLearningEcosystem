using UnityEngine;
using System.Collections.Generic;

//HAVE A FUNCTION WHERE IT RANDOMIZES THE CREATURE STATISTICS WITHOUT 
//EXCEEDING THE MAX AND MIN VALUES OF THE VARIABLES
public class CreatureStatstics : MonoBehaviour
{
    private Dictionary<string, float> stats = new();

    [SerializeField] private string _creatureName;

    void Start()
    {

        stats["Health"] = 0;
        stats["maxHealth"] = 0;

        stats["Speed"] = 0;
        stats["maxSpeed"] = 0;

        stats["Thirst"] = 0;
        stats["maxThirst"] = 0;

        stats["Hunger"] = 0;
        stats["maxHunger"] = 0;

        stats["Size"] = 0;
    }

    //setters and getters
    public string CreatureName
    {
        get { return _creatureName; }
        set { _creatureName = value; }
    }

    public float Health
    {
        get { return stats["Health"]; }
        set { stats["Health"] = value; }
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
    public float MaxSpeed
    {
        get { return stats["maxSpeed"]; }
        set { stats["maxSpeed"] = value; }
    }

    public float Thirst
    {
        get { return stats["Thirst"]; }
        set { stats["Thirst"] = value; }
    }
    public float MaxThirst
    {
        get { return stats["maxThirst"]; }
        set { stats["maxThirst"] = value; }
    }

    public float Hunger
    {
        get { return stats["Hunger"]; }
        set { stats["Hunger"] = value; }
    }
    public float MaxHunger
    {
        get { return stats["maxHunger"]; }
        set { stats["maxHunger"] = value; }
    }
  
    public float Size
    {
        get { return stats["Size"]; }
        set { stats["Size"] = value; }
    }

    public void RandomizeStats()
    {
        // Implement randomization logic here, ensuring that values do not exceed max and min limits
    }
}
