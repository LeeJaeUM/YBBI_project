using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestory <T> : MonoBehaviour where T : SingletonDontDestory<T>
{
    static T _instance = null;

    public static T Instance { get { return _instance; } }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = (T) this;
            OnAwake();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        OnStart();
    }
}
