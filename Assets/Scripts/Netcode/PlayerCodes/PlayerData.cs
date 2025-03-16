using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;


public class PlayerData : MonoBehaviour 
{
    public static PlayerData Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    string _playerID = "";
    bool _isReady = false;

    public PlayerData(bool IsReady, string playerID)
    {
        _playerID = playerID;
        _isReady = IsReady;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerID", _playerID },
            { "isReady", _isReady }
        };
    }

    public string GetPlayerID()
    {
        return _playerID;
    }
    public void SetPlayerID(string PlayerID)
    {
        _playerID = PlayerID;
    }
    public bool GetPlayerReady()
    {
        return _isReady;
    }
    public void SetPlayerReady(bool IsReady)
    {
        _isReady = IsReady;
    }

}