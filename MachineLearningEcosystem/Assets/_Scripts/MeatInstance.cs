using UnityEngine;

public class MeatInstance : MonoBehaviour
{
    private int bitesLeft = 3;
    public void Consume()
    {
        bitesLeft -= 1;
        if (bitesLeft <= 0)
        {
            MeatManager.instance.ClearMeat(gameObject);
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
