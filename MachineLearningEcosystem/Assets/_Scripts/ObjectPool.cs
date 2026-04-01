using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        T pooledObj = pool.Dequeue();
        pooledObj.gameObject.SetActive(true);
        return pooledObj;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
