using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{

    [SerializeField] private float movespeed = 1f;
    [SerializeField] private GameObject creature;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private FieldOfView fov;
    private CreatureStatstics stats;
    private void Awake()
    {
        stats = creature.GetComponent<CreatureStatstics>();
        if (stats == null)
        {
            stats = creature.AddComponent<CreatureStatstics>();
        }
    }

    private void Start()
    {
        fov.SetViewDistance(stats.ViewDistance);
        fov.UpdateViewDirection(0, stats.ViewAngle);
    }

    private void Update()
    {
        Move(Random.insideUnitCircle.normalized);
    }

    private bool Move(Vector2 direction)
    {
        direction = direction.normalized;
        Vector2 moveAttempt = movespeed * Time.deltaTime * direction;

        if (CheckBoundries(moveAttempt))
        {
            return false;
        }
        creature.transform.Translate(moveAttempt);
        return true;
    }

    private bool CheckBoundries(Vector2 moveAttempt)
    {
        Vector2 newPosition = moveAttempt + (Vector2)creature.transform.position;
        if (newPosition.x < 0
            || newPosition.x > width
            || newPosition.y < 0
            || newPosition.y > height)
        {
            return false;
        }
        return true;
    }

}
