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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void DisplayStats(CreatureStatstics creature)
    {
        NameText.text = creature.CreatureName;
        HealthText.text = "Health: " + creature.Health;
        HungerText.text = "Hunger: " + creature.Hunger;
        ThirstText.text = "Thirst: " + creature.Thirst;
        SpeedText.text = "Speed: " + creature.Speed;
    }

}