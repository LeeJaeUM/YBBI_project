using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour <T> : MonoBehaviour where T : SingletonMonoBehaviour <T>
{
    static T _instance = null;

    public static T Instance {  get { return _instance; } }
    
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
            _instance = (T)this;
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Instance == this)
        {
            OnStart();
        }
    }
}
