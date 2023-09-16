using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    public static PoolManager Inst { get; private set; }
    private Transform _parant;
    private Dictionary<string, Pool<PoolableMono>> _pools = new Dictionary<string, Pool<PoolableMono>>();

    public PoolManager(Transform parant)
    {
        _parant = parant;
        Inst = this;
    }

    public void CreatePool(PoolableMono prefab, int count = 10)
    {
        Pool<PoolableMono> pool = new Pool<PoolableMono>(prefab, _parant, count);
        _pools.Add(prefab.gameObject.name, pool);
    }

    public PoolableMono Pop(string prefabName)
    {
        if(!_pools.ContainsKey(prefabName))
        {
            Debug.LogError("Prefab doesnt exist on pool");
            return null;
        }

        PoolableMono item = _pools[prefabName].Pop();
        item.Reset();
        item.transform.SetParent(null);
        return item;
    }

    public void Push(PoolableMono obj)
    {
        obj.transform.SetParent(_parant);
        _pools[obj.name.Trim()].Push(obj);
    }
}
