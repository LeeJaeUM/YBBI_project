using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameObjectPool <T> where T : Component
{
    Queue<T> _pool;
    Func<T> _creatFunc;
    int _count;

    public int Count { get { return _pool.Count; } }

    public GameObjectPool(int count, Func<T> createFunc)
    {
        _count = count;
        _creatFunc = createFunc;
        _pool = new Queue<T>(count);
        Allocate();
    }

    public T Get()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }

        return _creatFunc();
    }

    public void Set (T obj)
    {
        _pool.Enqueue(obj);
    }

    void Allocate()
    {
        for (int i = 0; i < _count; i++)
        {
            _pool.Enqueue(_creatFunc());
        }
    }
}
