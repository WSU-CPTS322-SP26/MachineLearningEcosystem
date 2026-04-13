using UnityEngine;
using TMPro;

public class StatsBox : MonoBehaviour
{
    public static StatsBox Instance;

    [Header("Text Fields")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI HungerText;
    public TextMeshProUGUI ThirstText;
    public TextMeshProUGUI SpeedText;

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
        HealthText.text = "Health: " + creature.CurrHealth + "/" + creature.Health;
        HungerText.text = "Hunger: " + creature.CurrHunger + "/" + creature.Hunger;
        ThirstText.text = "Thirst: " + creature.CurrThirst + "/" + creature.Thirst;
        SpeedText.text = "Speed: " + creature.Speed;
    }

}