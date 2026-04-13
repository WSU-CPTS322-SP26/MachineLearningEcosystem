using UnityEngine;

public class MeatManager : MonoBehaviour
{
    [SerializeField] private GameObject meatPrefab;
    private ObjectPool<MeatInstance> meatPool;
    public static MeatManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        meatPool = new(meatPrefab.GetComponent<MeatInstance>(), 20, instance.gameObject.transform);
    }

    public void ClearMeat(GameObject meatObject)
    {
        meatPool.ReturnToPool(meatObject.GetComponent<MeatInstance>());
    }

    public void PlaceMeat(Vector3 pos, int bitesLeft)
    {
        MeatInstance newMeat = meatPool.Get();
        newMeat.transform.position = pos;
        newMeat.SetBitesLeft(bitesLeft);
    }
}
