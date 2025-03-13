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
    private List<PlayerData> playerList;
    public SessionData(string sessionName, string joinCode, bool isPrivate, string password)
    {
        SessionName = sessionName;
        JoinCode = joinCode;
        IsPrivate = isPrivate;
        Password = password;
        CurrentPlayers = 1; // 처음 생성 시 방장은 기본적으로 1명
        MaxPlayers = 4;

    }
}