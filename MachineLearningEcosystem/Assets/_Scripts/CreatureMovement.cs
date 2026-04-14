using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TerrainUtils;
using UnityEngine.Tilemaps;

public class CreatureMovement : MonoBehaviour
{
    [SerializeField] private float movespeed = 1f;
    [SerializeField] private GameObject creature;
    [SerializeField] private FieldOfView fov;

    private List<GameObject> vision;
    public CreatureStatstics stats;

    // --- Neural Network Brain ---
    public int CreatureId { get; private set; }    // unique ID for elite tracking
    private SharedBrain brain;                      // reference to the shared network
    private MLRewardCalculator rewardCalc = new MLRewardCalculator();

    // --- State flags (reset each step) ---
    public bool JustAte { get; private set; }
    public bool JustDrank { get; private set; }
    public bool IsDead { get; private set; }
    public bool JustDied { get; private set; }

    // --- Reward shaping: track distances before acting ---
    public float DistanceToNearestFood { get; private set; }
    public float DistanceToNearestWater { get; private set; }

    // --- State size constants ---
    // Vision encodes each detected object as:
    //   [normalizedDistance, isFood, isMeat, isWater, isPredator]
    //   We allow up to MAX_VISIBLE_OBJECTS objects in the FOV
    private const int MAX_VISIBLE_OBJECTS = 10;
    private const int FEATURES_PER_OBJECT = 5;
    private const int INTERNAL_STATS_COUNT = 3; // hunger, thirst, fov direction
    // Total input size = (10 * 5) + 3 = 53
    public const int STATE_SIZE = MAX_VISIBLE_OBJECTS * FEATURES_PER_OBJECT + INTERNAL_STATS_COUNT;
    public const int ACTION_SIZE = 6;


    //creature action to be able to get what action was preformed 
    public enum CreatureAction
    {
        MoveRandom = 0,
        MoveToward = 1,
        TurnLeft = 2,
        TurnRight = 3,
        Eat = 4,
        Drink = 5
    }



    private void Awake()
    {
        stats = creature.GetComponent<CreatureStatstics>();
        if (stats == null)
            stats = creature.AddComponent<CreatureStatstics>();

        stats.DeathSignal += DeathScript;
    }

    private void Start()
    {
        fov.SetViewDistance(stats.ViewDistance);
        fov.UpdateViewDirection(0, stats.ViewAngle);
        fov.SetCreature(creature);
    }

    // Called by SimulationManager when this creature is re-registered
    // after a respawn so it gets a fresh ID and brain reference
    public void Initialize(int id, SharedBrain sharedBrain)
    {
        CreatureId = id;
        brain = sharedBrain;
        IsDead = false;  // make sure dead flag is cleared
    }


    // Resets hunger, thirst and flags back to starting values
    // Called by CreatureRepopulationHandler before re-activating
    public void ResetCreature()
    {
        IsDead = false;
        JustDied = false;
        JustAte = false;
        JustDrank = false;

        // Reset stats to full
        stats.CurrHunger = stats.Hunger;
        stats.CurrThirst = stats.Thirst;

        DistanceToNearestFood = float.MaxValue;
        DistanceToNearestWater = float.MaxValue;
    }


    // Called before deactivating so the update loop stops immediately
    // without waiting for the death signal
    public void ForceDeactivate()
    {
        IsDead = true;
    }

    private void Update()
    {
        // Guard: don't run if brain hasn't been assigned yet
        // This can happen in the first frame before Initialize() is called
        if (brain == null) return;

        //Debug.Log(Path.Combine(Application.persistentDataPath, "creature_{1}_actor.json"));
        // Reset per-frame flags
        JustAte = false;
        JustDrank = false;
        JustDied = false;

        if (IsDead) return;

        // --- 1. Get vision ---
        vision = fov.GetDetectedObjects();

        // --- 2. Record distances BEFORE acting (for reward shaping) ---
        float prevFoodDist = DistanceToNearestFood;
        float prevWaterDist = DistanceToNearestWater;
        UpdateNearestDistances();

        // --- 3. Build state observation ---
        float[] state = GetObservation();

        // --- 4. Brain chooses action ---
        var (actionIndex, logProb, value) = brain.SelectAction(state);
        PerformAction(actionIndex);

        // --- 5. Calculate reward ---
        //float reward = rewardCalc.CalculateReward(this, actionIndex, prevFoodDist, prevWaterDist);
        //removed action index because it is not used in calculating reward
        float reward = rewardCalc.CalculateReward(this, prevFoodDist, prevWaterDist);

        // --- 6. Store experience for learning ---
        brain.StoreExperience(new Experience
        {
            State = state,
            ActionTaken = actionIndex,
            Reward = reward,
            NextState = GetObservation(),
            Done = IsDead,
            LogProbability = logProb,
            ValueEstimate = value,
            CreatureId = CreatureId     // the only new field
        }, CreatureId, reward);
    }

    // --- Build the observation vector from vision + internal stats ---
    public float[] GetObservation()
    {
        float[] obs = new float[STATE_SIZE];
        int idx = 0;

        // Fill in visible objects (pad with zeros if fewer than MAX_VISIBLE_OBJECTS)
        int objectCount = Mathf.Min(vision.Count, MAX_VISIBLE_OBJECTS);
        for (int i = 0; i < MAX_VISIBLE_OBJECTS; i++)
        {
            if (i < objectCount && vision[i] != null)
            {
                GameObject obj = vision[i];
                float dist = Vector2.Distance(transform.position, obj.transform.position);

                // Normalize distance by view distance so it's always 0-1
                obs[idx + 0] = Mathf.Clamp01(dist / stats.ViewDistance);
                obs[idx + 1] = obj.GetComponent<PlantInstance>() != null ? 1f : 0f; // isPlantFood
                obs[idx + 2] = obj.GetComponent<MeatInstance>() != null ? 1f : 0f; // isMeatFood
                obs[idx + 3] = IsWaterNearObject(obj) ? 1f : 0f; // isWater
                obs[idx + 4] = obj.GetComponent<CreatureMovement>() != null &&
                               obj != this.gameObject ? 1f : 0f; // isOtherCreature
            }
            // else: already zero-padded by default
            idx += FEATURES_PER_OBJECT;
        }

        // Internal stats (all normalized 0-1)
        obs[idx + 0] = Mathf.Clamp01(stats.CurrHunger / stats.Hunger);
        obs[idx + 1] = Mathf.Clamp01(stats.CurrThirst / stats.Thirst);
        obs[idx + 2] = Mathf.Clamp01(fov.GetViewDirection() / 360f);

        return obs;
    }

    // --- Execute whichever action the brain chose ---
    private void PerformAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0: // Move in a random direction (explore)
                Move(Random.insideUnitCircle.normalized, 1f);
                break;

            case 1: // Move toward the nearest relevant object in vision
                MoveTowardTarget();
                break;

            case 2: // Turn left
                UpdateFov((int)fov.GetViewDirection() - 15);
                break;

            case 3: // Turn right
                UpdateFov((int)fov.GetViewDirection() + 15);
                break;

            case 4: // Eat
                Eat();
                break;

            case 5: // Drink
                Drink();
                break;
        }
    }

    // --- Move toward the highest-priority visible object ---
    // Priority: food (matching diet) > water (if thirsty) > anything else
    private void MoveTowardTarget()
    {
        GameObject target = null;
        float bestScore = float.MinValue;

        foreach (GameObject obj in vision)
        {
            if (obj == null) continue;

            float score = 0f;
            float dist = Vector2.Distance(transform.position, obj.transform.position);
            float normDist = Mathf.Clamp01(dist / stats.ViewDistance);

            bool isFood = stats.IsCarnivore
                ? obj.GetComponent<MeatInstance>() != null
                : obj.GetComponent<PlantInstance>() != null;

            bool isWater = IsWaterNearObject(obj);

            // Score food higher when hungry, water higher when thirsty
            if (isFood)
                score = (1f - stats.CurrHunger / stats.Hunger) * 10f - normDist;
            else if (isWater)
                score = (1f - stats.CurrThirst / stats.Thirst) * 8f - normDist;

            if (score > bestScore)
            {
                bestScore = score;
                target = obj;
            }
        }

        if (target != null)
        {
            Vector2 dir = ((Vector2)target.transform.position
                         - (Vector2)transform.position).normalized;
            Move(dir, 1f);
        }
        else
        {
            // Nothing useful visible, just move forward in current fov direction
            float angle = fov.GetViewDirection() * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Move(dir, 1f);
        }
    }

    // --- Check if a game object is near a water tile ---
    private bool IsWaterNearObject(GameObject obj)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(obj.transform.position, 0.6f);
        foreach (Collider2D col in colliders)
        {
            MapTerrain tile = col.GetComponent<MapTerrain>();
            if (tile != null && tile.GetTerrainData()?.GetTerrainType() == "water")
                return true;
        }
        return false;
    }

    // --- Update nearest food/water distances for reward shaping ---
    private void UpdateNearestDistances()
    {
        float nearestFood = float.MaxValue;
        float nearestWater = float.MaxValue;

        foreach (GameObject obj in vision)
        {
            if (obj == null) continue;
            float dist = Vector2.Distance(transform.position, obj.transform.position);

            bool isFood = stats.IsCarnivore
                ? obj.GetComponent<MeatInstance>() != null
                : obj.GetComponent<PlantInstance>() != null;

            if (isFood && dist < nearestFood)
                nearestFood = dist;

            if (IsWaterNearObject(obj) && dist < nearestWater)
                nearestWater = dist;
        }

        DistanceToNearestFood = nearestFood;
        DistanceToNearestWater = nearestWater;
    }

    public void Eat()
    {
        if (stats.IsCarnivore)
        {
            GameObject target = CanEatMeat();
            if (target != null)
            {
                stats.CurrHunger += 40f;
                JustAte = true;
                target.GetComponent<MeatInstance>()?.Consume();
            }
        }
        else
        {
            GameObject target = CanEatPlants();
            if (target != null)
            {
                stats.CurrHunger += 40f;
                JustAte = true;
                target.GetComponent<PlantInstance>()?.Consume();
            }
        }
    }
    public GameObject CanEatMeat()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); // give creature some eating range
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<MeatInstance>() != null)
            {
                return collider.gameObject;
            }
        }
        return null;
    }
    public GameObject CanEatPlants()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); // give creature some eating range
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<PlantInstance>() != null)
            {
                return collider.gameObject;
            }
        }
        return null;
    }

    public void Drink()
    {
        if (CanDrink())
        {
            JustDrank = true;
            stats.CurrThirst += 20f;
        }
    }
    public bool CanDrink()
    {
        // Detect tiles in each of four directions, you can use an action to drink if one of them is water
        // Salt water does not exist in this world (or everything can drink salt, idk)
        List<MapTerrain> ns = GetNeighbors(0.5f);
        foreach (MapTerrain tile in ns)
        {
            if (tile != null && tile.GetTerrainData().GetTerrainType() == "water")
            {
                return true;
            }
        }
        return false;
    }
    // Get neighboring tiles (if distanceforcheck == 1), if distanceforcheck < 1, get closeby tiles (creature is at edge of one tile)
    private List<MapTerrain> GetNeighbors(float distanceForCheck)
    {
        MapTerrain currentTile = DetectTile(transform.position);
        List<MapTerrain> ns = new();
        ns.Add(DetectTile(transform.position + (Vector3.up * MapManager.GetTerrainSize() * distanceForCheck)));
        ns.Add(DetectTile(transform.position + (Vector3.up * -1 * MapManager.GetTerrainSize() * distanceForCheck)));
        ns.Add(DetectTile(transform.position + (Vector3.right * MapManager.GetTerrainSize() * distanceForCheck)));
        ns.Add(DetectTile(transform.position + (Vector3.right * -1 * MapManager.GetTerrainSize() * distanceForCheck)));
        return ns;
    }
    
    // Get what the creature sees
    public List<GameObject> GetVision()
    {
        return vision;
    }

    // Change where the creature is looking currently
    public void UpdateFov(int v)
    {
        fov.UpdateViewDirection(v, stats.ViewAngle);
    }

    public MapTerrain GetCurrentTile()
    {
        return DetectTile(transform.position);
    }

    // Get the tile at the target position
    public MapTerrain DetectTile(Vector3 position)
    {
        MapTerrain tile = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        // Process the detected colliders to determine the current tile
        foreach (Collider2D collider in colliders)
        {
            tile = collider.GetComponent<MapTerrain>();
            if (tile != null)
            {
                break;
            }
        }
        return tile;
    }

    // Move in the direction a percent of the creature's speed value
    public bool Move(Vector2 direction, float amount)
    {
        amount = Mathf.Clamp01(amount); // The percentage of their speed the creature moves, so they dont get trapped
        direction = direction.normalized;
        Vector2 moveAttempt = amount * movespeed * Time.deltaTime * direction;

        if (!CheckBoundries(moveAttempt))
        {
            return false;
        }
        gameObject.transform.Translate(moveAttempt);
        return true;
    }

    // Helper function to confirm if a move is valid
    private bool CheckBoundries(Vector2 moveAttempt)
    {
        Vector2 newPosition = moveAttempt + (Vector2)gameObject.transform.position;
        MapTerrain tile = DetectTile(newPosition);
        if (tile == null || tile.GetTerrainData() == null || tile.GetTerrainData().GetTerrainType() == "water" || tile.GetTerrainData().GetTerrainType() == "Empty")
        {
            return false; // cannot move: must have a tile
        }
        return true;
    }

    private void OnDestroy()
    {
        if (stats != null)
        {
            stats.DeathSignal -= DeathScript;
        }
    }

    // TODO: Add to this function all important effects that happen when a creature dies (drop meat to eat, alert the ML system, etc.)
    private void DeathScript()
    {
        JustDied = true;
        MeatManager.instance?.PlaceMeat(gameObject.transform.position, Random.Range(3,6));
        Destroy(gameObject);
    }
}
