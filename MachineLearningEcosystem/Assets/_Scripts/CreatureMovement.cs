using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TerrainUtils;

public class CreatureMovement : MonoBehaviour
{

    [SerializeField] private float movespeed = 1f;
    [SerializeField] private GameObject creature;
    // [SerializeField] private int width;
    // [SerializeField] private int height;
    [SerializeField] private FieldOfView fov;
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
        MapTerrain currentTile = DetectTile(gameObject.transform.position);
        Move(Random.insideUnitCircle.normalized, 1f);
        UpdateFov((int)fov.GetViewDirection() + Random.Range(-5, 6));
    }

    public List<GameObject> GetVision()
    {
        return fov.GetDetectedObjects(); // Track what it sees!
    }
    public void UpdateFov(int v)
    {
        fov.UpdateViewDirection(v, stats.ViewAngle);
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
        // if (newPosition.x < 0
        //     || newPosition.x > width
        //     || newPosition.y < 0
        //     || newPosition.y > height)
        // {
        //     Debug.Log("Cannot move to target: Out of bounds");
        //     return false;
        // }
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
        Destroy(gameObject);
    }
}
