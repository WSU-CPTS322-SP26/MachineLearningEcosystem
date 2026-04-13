using UnityEngine;
using TMPro;

public class StatsBox : MonoBehaviour
{
    public static StatsBox Instance;

    [Header("Text Fields")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI StatsText;

    public GameObject save_creature = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Update()
    {
        if (save_creature != null)
        {
            DisplayStats(save_creature.GetComponent<CreatureStatstics>());
        }
    }

    public void DisplayStats(CreatureStatstics creature)
    {
        save_creature = creature.gameObject;
        NameText.text = creature.CreatureName;
        StatsText.text = "Health: " + creature.CurrHealth + "/" + creature.Health;
        StatsText.text += "\nHunger: " + creature.CurrHunger + "/" + creature.Hunger;
        StatsText.text += "\nThirst: " + creature.CurrThirst + "/" + creature.Thirst;
        StatsText.text += "\nSpeed: " + creature.Speed;
        StatsText.text += "\nDamage: " + creature.Damage;
        StatsText.text += "\nInteraction Range: " + creature.Range + " Units";
        StatsText.text += "\nView Distance: " + creature.ViewDistance + " Units";
        StatsText.text += "\nField of View: " + creature.ViewAngle + " Units";
    }

}