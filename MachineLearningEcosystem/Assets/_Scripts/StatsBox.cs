using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsBox : MonoBehaviour
{
    public static StatsBox Instance;

    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI StatsText;
    [SerializeField] private Button closeButton;
    private static AudioClip openSound = null;
    private static AudioClip closeSound = null;
    private bool displaying = false;


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
        openSound = Resources.Load<AudioClip>("SFX/Inspect");
        closeSound = Resources.Load<AudioClip>("SFX/CloseInspect");
        // disable without sfx reference
        displaying = false;
        save_creature = null;
        gameObject.GetComponent<Image>().enabled = false;
        NameText.gameObject.SetActive(false);
        StatsText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // closeButton.onClick.RemoveAllListeners();
    }

    public void Update()
    {
        if (save_creature != null)
        {
            DisplayStats(save_creature.GetComponent<CreatureStatistics>());
        }
    }

    public void DisableBox()
    {
        displaying = false;
        save_creature = null;
        gameObject.GetComponent<Image>().enabled = false;
        NameText.gameObject.SetActive(false);
        StatsText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        if (closeSound != null)
        {
            SFXManager.instance.PlayOmnicientAudioClip(closeSound);
        }
    }

    public void EnableBox()
    {
        displaying = true;
        gameObject.GetComponent<Image>().enabled = true;
        NameText.gameObject.SetActive(true);
        StatsText.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void DisplayStats(CreatureStatistics creature)
    {
        if (!displaying)
            EnableBox();
        if (save_creature != creature.gameObject)
        {
            save_creature = creature.gameObject;
            NameText.text = creature.CreatureName;
            if (openSound != null)
            {
                SFXManager.instance.PlayOmnicientAudioClip(openSound);
            }
        }
        if (creature.IsCarnivore)
        {
            StatsText.text = "Type: Carnivore";
        }
        else
        {
            StatsText.text = "Type: Herbivore";
        }
        StatsText.text += "\nHealth: " + (int)creature.CurrHealth + "/" + (int)creature.Health;
        StatsText.text += "\nHunger: " + (int)creature.CurrHunger + "/" + (int)creature.Hunger;
        StatsText.text += "\nThirst: " + (int)creature.CurrThirst + "/" + (int)creature.Thirst;
        StatsText.text += "\nSpeed: " + creature.Speed.ToString("F2");
        StatsText.text += "\nDamage: " + creature.Damage.ToString("F2");
        StatsText.text += "\nRange: " + creature.Range.ToString("F2") + " u";
        StatsText.text += "\nView Dist: " + creature.ViewDistance.ToString("F2") + " u";
        StatsText.text += "\nFov: " + creature.ViewAngle.ToString("F2") + " deg";
    }

}