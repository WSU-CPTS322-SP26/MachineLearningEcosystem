using System.Collections.Generic;
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
    // [SerializeField] private int width;
    // [SerializeField] private int height;
    [SerializeField] private FieldOfView fov;
    private List<GameObject> vision;
    private CreatureStatstics stats;
    
    private void Awake()
    {
        stats = creature.GetComponent<CreatureStatstics>();
        if (stats == null)
        {
            stats = creature.AddComponent<CreatureStatstics>();
        }
        stats.DeathSignal += DeathScript;
    }

    private void Start()
    {
        fov.SetViewDistance(stats.ViewDistance);
        fov.UpdateViewDirection(0, stats.ViewAngle);
        fov.SetCreature(creature);
    }

    private void Update()
    {
        vision = fov.GetDetectedObjects();
        Move(Random.insideUnitCircle.normalized, 1f);
        UpdateFov((int)fov.GetViewDirection() + Random.Range(-5, 6));
        Drink();
        Eat();
    }

    public void Eat()
    {
        if (stats.IsCarnivore)
        {
            GameObject target = CanEatMeat();
            if (target != null)
            {
                stats.CurrHunger += 40f;
                target.GetComponent<MeatInstance>()?.Consume();
            }
        }
        else
        {
            GameObject target = CanEatPlants();
            if (target != null)
            {
                stats.CurrHunger += 40f;
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
        MeatManager.instance?.PlaceMeat(gameObject.transform.position, Random.Range(3,6));
        Destroy(gameObject);
    }
}
