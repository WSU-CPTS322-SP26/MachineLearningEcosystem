using UnityEngine;
using System.Collections.Generic;

//HAVE A FUNCTION WHERE IT RANDOMIZES THE CREATURE STATISTICS WITHOUT 
//EXCEEDING THE MAX AND MIN VALUES OF THE VARIABLES
public class CreatureStatstics : MonoBehaviour
{
    private Dictionary<string, float> stats = new Dictionary<string, float>();

    private string _creatureName;

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
    public string creatureName
    {
        get { return _creatureName; }
        set { _creatureName = value; }
    }

    public float Health
    {
        get { return stats["Health"]; }
        set { stats["Health"] = value; }
    }
    public float maxHealth
    {
        get { return stats["maxHealth"]; }
        set { stats["maxHealth"] = value; }
    }

    public float Speed
    {
        get { return stats["Speed"]; }
        set { stats["Speed"] = value; }
    }
    public float maxSpeed
    {
        get { return stats["maxSpeed"]; }
        set { stats["maxSpeed"] = value; }
    }

    public float Thirst
    {
        get { return stats["Thirst"]; }
        set { stats["Thirst"] = value; }
    }
    public float maxThirst
    {
        get { return stats["maxThirst"]; }
        set { stats["maxThirst"] = value; }
    }

    public float Hunger
    {
        get { return stats["Hunger"]; }
        set { stats["Hunger"] = value; }
    }
    public float maxHunger
    {
        get { return stats["maxHunger"]; }
        set { stats["maxHunger"] = value; }
    }
  
    public float Size
    {
        get { return stats["Size"]; }
        set { stats["Size"] = value; }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
