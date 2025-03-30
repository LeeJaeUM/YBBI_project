using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class FireBaseSessionData
{
    public string SessionName;
    public string JoinCode;
    public string Password;
    public bool IsPrivate;
    public bool IsStart;
    public int CurrentPlayers; // 현재 접속 인원
    public int MaxPlayers; // 최대 접속 가능 인원
    public List<PlayerData> Players;
    public FireBaseSessionData(bool isStart,string sessionName, string joinCode, bool isPrivate, string password, int currentPlayer,List<PlayerData> newPlayerList)
    {
        IsStart = isStart;
        SessionName = sessionName;
        JoinCode = joinCode;
        IsPrivate = isPrivate;
        Password = password;
        CurrentPlayers = currentPlayer;
        MaxPlayers = 4;
        Players = newPlayerList;
    }

    public bool GetIsStartInSessionData()
    {
        return IsStart;
    }
    public void AddCurrentPlayers(int AddPlayerNum)
    {
        CurrentPlayers += AddPlayerNum;
    }
}