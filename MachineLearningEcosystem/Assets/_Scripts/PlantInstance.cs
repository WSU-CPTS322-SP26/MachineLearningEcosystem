using UnityEngine;
using UnityEngine.UIElements.Experimental;

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
    public int GetBitesLeft()
    {
        return bitesLeft;
    }
    public void SetBitesLeft(int value)
    {
        bitesLeft = value;
    }
}
