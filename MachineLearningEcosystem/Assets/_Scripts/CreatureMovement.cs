using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{

    [SerializeField] private float moveTime = 0;
    private float timer;


    [SerializeField] private GameObject creature;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        new WaitForSeconds(1f);
    }


    private void Move()
    {
        if (timer >= moveTime)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            creature.transform.Translate(direction * 1);
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
}
