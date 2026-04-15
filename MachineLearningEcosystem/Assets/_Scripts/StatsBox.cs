using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsBox : MonoBehaviour
{
    public static StatsBox Instance;

    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI StatsText;
    [SerializeField] private Button closeButton;

    private GameObject save_creature = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // closeButton.onClick.AddListener(DisableBox);
        DisableBox();
    }

    private void OnDestroy()
    {
        // closeButton.onClick.RemoveAllListeners();
    }

    public void Update()
    {
        if (save_creature != null)
        {
            DisplayStats(save_creature.GetComponent<CreatureStatstics>());
        }
    }

    public void DisableBox()
    {
        save_creature = null;
        gameObject.GetComponent<Image>().enabled = false;
        NameText.gameObject.SetActive(false);
        StatsText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void EnableBox()
    {
        gameObject.GetComponent<Image>().enabled = true;
        NameText.gameObject.SetActive(true);
        StatsText.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void DisplayStats(CreatureStatstics creature)
    {
        EnableBox();
        save_creature = creature.gameObject;
        NameText.text = creature.CreatureName;
        if (creature.IsCarnivore)
        {
            StatsText.text = "Type: Carnivore";
        }
        else
        {
            StatsText.text = "Type: Herbivore";
        }
        StatsText.text += "\nHealth: " + creature.CurrHealth + "/" + creature.Health;
        StatsText.text += "\nHunger: " + creature.CurrHunger + "/" + creature.Hunger;
        StatsText.text += "\nThirst: " + creature.CurrThirst + "/" + creature.Thirst;
        StatsText.text += "\nSpeed: " + creature.Speed;
        StatsText.text += "\nDamage: " + creature.Damage;
        StatsText.text += "\nAction Range: " + creature.Range + " Units";
        StatsText.text += "\nView Distance: " + creature.ViewDistance + " Units";
        StatsText.text += "\nField of View: " + creature.ViewAngle + " Units";
    }

}