using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<string, object> _pools = new Dictionary<string, object>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePool<T>(string key, T prefab, int initialSize) where T : Component
    {
        if (_pools.ContainsKey(key)) return;
        _pools[key] = new ObjectPool<T>(prefab, initialSize);
    }

    public T Get<T>(string key) where T : Component
    {
        if (_pools.TryGetValue(key, out object pool))
        {
            return ((ObjectPool<T>)pool).Get();
        }

        Debug.LogWarning($"Pool with key {key} not found");
        return null;
    }

    public void Return<T>(string key, T obj) where T : Component
    {
        if (_pools.TryGetValue(key, out object pool))
        {
            ((ObjectPool<T>)pool).Return(obj);
        }
        else
        {
            Debug.LogWarning($"Pool with key {key} not found");
        }
    }
}
