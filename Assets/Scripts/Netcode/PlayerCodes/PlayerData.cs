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

    string _playerName = "";
    bool _isReady = false;

    public PlayerData(bool IsReady, string playerName)
    {
        _playerName = playerName;
        _isReady = IsReady;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "PlayerName", _playerName },
            { "IsReady", _isReady }
        };
    }

    public string GetPlayerName()
    {
        return _playerName;
    }
    public void SetPlayerName(string PlayerName)
    {
        _playerName = PlayerName;
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