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
    void Awake()
    {
        stats = creature.GetComponent<CreatureStatstics>();
        if (stats == null)
        {
            stats = creature.AddComponent<CreatureStatstics>();
        }

        fov.SetViewAngle(50);
    }

    void Update()
    {
        Move(Random.insideUnitCircle.normalized);
    }

    private bool Move(Vector2 direction)
    {
        direction = direction.normalized;
        Vector2 moveAttempt = direction * movespeed * Time.deltaTime;

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
