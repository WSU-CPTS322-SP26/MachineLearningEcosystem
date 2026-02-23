using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{

    [SerializeField] private float moveTime = 0;
    private float timer;


    [SerializeField] private GameObject creature;
    [SerializeField] private int width;
    [SerializeField] private int height;



    //doesn't work when pausing in simulation (doesn't restart after unpause) and doesn't start when entering from main menu


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private void Move()
    {
        if (timer >= moveTime)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector2 moveAttempt = direction * 2;

            while (!CheckBoundries(moveAttempt))
            {
                direction = Random.insideUnitCircle.normalized;
                moveAttempt = direction * 2;
            }
            creature.transform.Translate(moveAttempt);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
        

       

    }

    private void Move(Vector2 direction)
    {

    }

    private bool CheckBoundries(Vector2 moveAttempt)
    {
        if (moveAttempt.x + creature.transform.position.x < 0 || moveAttempt.x + creature.transform.position.x > width || moveAttempt.y + creature.transform.position.y  < 0 || moveAttempt.y + creature.transform.position.y > height)
        {
            return false;
        }
        return true;
    }

}
