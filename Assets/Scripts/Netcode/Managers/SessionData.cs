using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class SessionData
{
    public string SessionName;
    public string JoinCode;
    public bool IsPrivate;
    public string Password;
    public int CurrentPlayers; // 현재 접속 인원
    public int MaxPlayers; // 최대 접속 가능 인원
    public List<PlayerData> Players;
    public SessionData(string sessionName, string joinCode, bool isPrivate, string password, int currentPlayer,List<PlayerData> newPlayerList)
    {
        SessionName = sessionName;
        JoinCode = joinCode;
        IsPrivate = isPrivate;
        Password = password;
        CurrentPlayers = currentPlayer;
        MaxPlayers = 4;
        Players = newPlayerList;
    }

    public void AddCurrentPlayers(int AddPlayerNum)
    {
        CurrentPlayers += AddPlayerNum;
    }
}