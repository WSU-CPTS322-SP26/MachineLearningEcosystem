using UnityEngine;

public class PlantInstance : MonoBehaviour
{
    private int bitesLeft = 3;

    public void Consume()
    {
        bitesLeft -= 1;
        if (bitesLeft <= 0)
        {
            PlantManager.instance.ClearPlant(gameObject);
        }
    }
}
