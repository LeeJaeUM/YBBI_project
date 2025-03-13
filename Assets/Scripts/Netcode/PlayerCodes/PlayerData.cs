using System.Net;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public static PlayerData LocalInstance { get; private set; }
    public static PlayerData Instance { get; private set; }

    string _playerID = "";
    bool _IsReady = false;

    private void Awake()
    {
        if (IsOwner)
        {
            Instance = this;
        }

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


    public PlayerData(bool IsReady, string playerID)
    {
        _playerID = playerID;
        _IsReady = IsReady;
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
        return _IsReady;
    }
    public void SetPlayerReady(bool IsReady)
    {
        _IsReady = IsReady;
    }

}