using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using System;
using WebSocketSharp;
using System.Security.Cryptography;

public class NetcodeFireBaseManager : MonoBehaviour
{
    [SerializeField] private const int MaxConnections = 4;

    public static NetcodeFireBaseManager Instance;
    private DatabaseReference dbReference;

    public int _currentPlayers = 0;

    private string firebaseDatabaseUrl = "https://mytest-10314-default-rtdb.firebaseio.com/";

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
        dbReference = FirebaseDatabase.GetInstance(firebaseDatabaseUrl).RootReference;

    }
    void Start()
    {

        FirebaseDatabase.DefaultInstance.GetReference("sessions").ValueChanged += OnSessionListChanged;
        FirebaseDatabase.DefaultInstance.GetReference("Players").ValueChanged += OnPlayerListChanged;
    }




    public DatabaseReference GetDBreference()
    {
        return dbReference;
    }
    public int GetMaxConnection()
    {
        return MaxConnections;
    }

    public void AddFireBaseSession(string sessionId, SessionData newSession)
    {
        dbReference.Child("sessions").Child(sessionId).SetRawJsonValueAsync(JsonUtility.ToJson(newSession));
    }

    public List<PlayerData> newPlayerListMaker()
    {
        List<PlayerData> newList = new List<PlayerData>();
        for (int i = 0; i < 4; i++)
        {
            newList.Add(new PlayerData(false, i.ToString(), 0));
        }

        return newList;
    }


    public async Task<bool> SetCurrentPlayer(int currentNum, string joinCode)
    {
        // 🔥 세션 ID 가져오기
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError($"세션 ID를 찾을 수 없습니다. JoinCode: {joinCode}");
            return false;
        }

        // 🔥 Firebase에서 현재 플레이어 수 가져오기
        int currentPlayers = await GetCurrentPlayer(joinCode);
        currentPlayers += currentNum; // 새로운 플레이어 추가

        // 🔥 Null 값 방지 (Firebase는 Null을 허용하지 않음)
        if (currentPlayers < 0)
        {
            currentPlayers = 0;
        }

        // 🔥 Firebase 업데이트
        await dbReference.Child("sessions").Child(sessionId).Child("CurrentPlayers").SetValueAsync(currentPlayers);

        Debug.Log($"세션 {sessionId}의 현재 플레이어 수: {currentPlayers}");
        return true;
    }

    public async Task<int> GetCurrentPlayer(string joinCode)
    {
        Debug.Log("GetcurrentPlayer함수 실행");
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        Debug.Log($"SessionID : {sessionId}");
        if (sessionId == null)
        {
            Debug.LogError("세션 ID를 찾을 수 없습니다.");
            return 0;
        }

        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("CurrentPlayers").GetValueAsync();
        if (snapshot.Exists)
        {
            Debug.Log("GetcurrentPlayer실행 성공적");
            return int.Parse(snapshot.Value.ToString());
        }

        return 0;
    }



    public async Task<bool> AddPlayer(string joinCode)
    {
        Debug.Log("플레이어 추가 시도");


        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("세션 ID를 찾을 수 없습니다.");
            return false;
        }

        int playerIndex = -1;

        // Players 경로에서 모든 데이터 가져오기
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").GetValueAsync();
        if (snapshot.Exists)
        {
            int index = 0;
            foreach (var child in snapshot.Children)
            {
                string currentPlayerName = child.Child("PlayerName").Value?.ToString(); // "playerID" 필드를 가져옴
                if (currentPlayerName == null)
                {
                    Debug.Log("foreach문안의 if문 작동");
                    playerIndex = index;
                    break;
                }
                index++;
            }
        }

        if (playerIndex == -1)
        {
            Debug.LogError("플레이어 index 찾기 실패");
            return false;
        }

        string playerName = $"{joinCode}__{playerIndex}";
        Debug.Log($"player이름 : {playerName}");
        PlayerData newPlayer = new PlayerData(false, playerName, 0); //임시로 플레이어 네임을 아무렇게나 넣엇음
        UIManager.Instance.SetSavedPlayerName(playerName);


        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).SetValueAsync(newPlayer.ToDictionary());
        await SetCurrentPlayer(1, joinCode);

        Debug.Log($"세션 {sessionId}에 {playerIndex}번째 플레이어 추가 완료!");

        //UIManager.Instance.UpdatePlayerPanels();

        return true;
    }



    public async Task<bool> RemovePlayerFromSession(string joinCode, string playerName)
    {
        Debug.Log("플레이어 제거 시도");

        // JoinCode로 SessionID 찾기
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("세션을 찾을 수 없습니다.");
            return false;
        }

        Debug.Log($"제거할 플레이어가 있는 세션 ID: {sessionId}");

        int playerIndex = -1;

        // Players 경로에서 모든 데이터 가져오기
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").GetValueAsync();
        if (snapshot.Exists)
        {
            int index = 0;
            foreach (var child in snapshot.Children)
            {
                
                string currentPlayerName = child.Child("PlayerName").Value?.ToString(); // "playerID" 필드를 가져옴
                Debug.Log($"{index}번째 플레이어 이름 : {currentPlayerName}, 현재 찾는 플레이어 이름 : {playerName}");
                if (currentPlayerName == playerName)
                {
                    Debug.Log("foreach문안의 if문 작동");
                    playerIndex = index;
                    Debug.Log($"{sessionId} 내의 {playerName}의 인덱스: {playerIndex}");
                    break;
                }
                index++;
            }
        }

        if (playerIndex == -1)
        {
            Debug.LogError("플레이어 index 가져오기 실패");
            return false;
        }

        // 플레이어 데이터 제거
        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).RemoveValueAsync();

        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("instanceID").SetValueAsync(0);

        Debug.Log($"플레이어 {playerName} 제거 완료");

        // 현재 플레이어 수 업데이트
        await SetCurrentPlayer(-1, joinCode);

        return true;
    }
    public async void RequseReversalReadyToggle(string joincode, string playerName)
    {
        await ReversalReadyToggle(joincode, playerName);
    }

    private async Task<bool> ReversalReadyToggle(string joinCode, string playerName)
    {
        string sessionId = await GetSessionIdByJoinCode(joinCode);

        int playerIndex = -1;

        // Players 경로에서 모든 데이터 가져오기
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").GetValueAsync();
        if (snapshot.Exists)
        {
            int index = 0;
            foreach (var child in snapshot.Children)
            {

                string currentPlayerName = child.Child("PlayerName").Value?.ToString(); // "playerID" 필드를 가져옴
                Debug.Log($"{index}번째 플레이어 이름 : {currentPlayerName}, 현재 찾는 플레이어 이름 : {playerName}");
                if (currentPlayerName == playerName)
                {
                    Debug.Log("foreach문안의 if문 작동");
                    playerIndex = index;
                    Debug.Log($"{sessionId} 내의 {playerName}의 인덱스: {playerIndex}");
                    break;
                }
                index++;
            }
        }

        if (playerIndex == -1)
        {
            Debug.LogError("플레이어 index 가져오기 실패");
            return false;
        }

        snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("IsReady").GetValueAsync();
        if (snapshot.Exists)
        {
            Debug.Log($"{playerIndex}번째 플레이어 준비 상태 변경 시도 현재 : {snapshot.Value}");

            if(snapshot.Value.ToString() == false.ToString())
            {
                await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("IsReady").SetValueAsync(true);
                Debug.Log($"{playerIndex}번째 플레이어 준비 상태 변경 성공 현재 : {snapshot.Value}");
            }
            else if(snapshot.Value.ToString() == true.ToString())
            {
                await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("IsReady").SetValueAsync(false);
                Debug.Log($"{playerIndex}번째 플레이어 준비 상태 변경 성공 현재 : {snapshot.Value}");
            }
            else
            {
                Debug.Log($"{playerIndex}번째 플레이어 준비 상태 변경 실패 현재 : {snapshot.Value}");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> GetSessionPlayerIsReady(string joinCode, int playerIndex)
    {
        string sessionId = await GetSessionIdByJoinCode(joinCode);

        if (string.IsNullOrEmpty(sessionId))
        {
            return false;
        }


        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("IsReady").GetValueAsync();
        if (snapshot.Exists)
        {
            Debug.Log($"{playerIndex}번째 플레이어 준비 상태 : {snapshot.Value}");
            if (snapshot.Value.ToString() == true.ToString())
            {
                return true;
            }

        }
        return false;
    }

    public async Task<string> GetSessionPlayerName(string joinCode, int playerIndex)
    {
        Debug.Log("플레이어 이름 가져오기 시도");
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if(string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("플레이어 이름 가져오기 에서 세션 아이디 찾기 실패");
            return "Error";
        }

        Debug.Log($"세션야이디 {sessionId} 의 {playerIndex}번쨰 플레이어 이름 가져오기 시도");
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("PlayerName").GetValueAsync();
        if (snapshot.Exists)
        {
            Debug.Log($"플레이어 이름 : {snapshot.Value}");
            return snapshot.Value.ToString();
        }

        Debug.Log("플레이어 이름 가져오기 마지막 단계 실패");
        return "Player" ;
    }

    public async Task<int> GetSessionPlayerJobIndex(string joinCode, int playerIndex)
    {
                Debug.Log("플레이어 직업 인덱스 가져오기 시도");
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if(string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("플레이어 직업 인덱스 가져오기 에서 세션 아이디 찾기 실패");
            return 0;
        }

        Debug.Log($"세션야이디 {sessionId} 의 {playerIndex}번쨰 플레이어 직업 인덱스 가져오기 시도");
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("PlayerJobIndex").GetValueAsync();
        if (snapshot.Exists)
        {
            Debug.Log($"플레이어 직업 인덱스 : {snapshot.Value}");
            return int.Parse(snapshot.Value.ToString());
        }

        Debug.Log("플레이어 이름 가져오기 마지막 단계 실패");

        return 0;
    }

    public async Task<string> GetSessionIdByJoinCode(string joinCode)
    {
        Debug.Log("키 요청");
        
        DataSnapshot snapshot = await dbReference.Child("sessions")
                                        .OrderByChild("JoinCode")
                                        .EqualTo(joinCode)
                                        .GetValueAsync();
        if (snapshot.Exists)
        {
            var sessionList = snapshot.Children.ToList();
            if (sessionList.Count > 0)
            {
                string sessionId = sessionList[0].Key.ToString();
                Debug.Log($"{joinCode}를 가진 세션 아이디 획득 성공 : {sessionId}");
                return sessionId;
            }
        }
        Debug.Log($"{joinCode}를 가진 세션 아이디 획득 실패");
        return null;
    }

    public async void RemoveSessionFromFirebase(string joinCode)
    {
        try
        {
            // Firebase에서 특정 JoinCode를 가진 세션 찾기
            Debug.Log("파이어베이스 세션 제거 시도");
            var sessionID = await GetSessionIdByJoinCode(joinCode);
            if (sessionID != null)
            {
                await dbReference.Child("sessions").Child(sessionID).RemoveValueAsync();
                Debug.Log("파이어베이스 세션 제거성공");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase에서 세션 삭제 실패: {e.Message}");
        }
    }

    public async void OnSessionListChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            RelayManager.Instance.GetSessionList().Clear();
            foreach (var child in e.Snapshot.Children)
            {
                try
                {
                    string sessionName = child.Child("SessionName").Value?.ToString() ?? "Unknown";
                    string joinCode = child.Child("JoinCode").Value?.ToString() ?? "";
                    bool isPrivate = bool.TryParse(child.Child("IsPrivate").Value?.ToString(), out bool parsedPrivate) && parsedPrivate;
                    string password = child.Child("Password").Value?.ToString() ?? "";
                    int currentPlayers = int.Parse(child.Child("CurrentPlayers").Value?.ToString() ?? "0"); 
                    List<PlayerData> Players = new List<PlayerData>();

                    RelayManager.Instance.GetSessionList().Add(new SessionData(sessionName, joinCode, isPrivate, password, currentPlayers, Players));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Firebase 데이터 처리 중 오류 발생: {ex.Message}");
                }
            }
        }
        else
        {
            RelayManager.Instance.GetSessionList().Clear();
        }

       
        UIManager.Instance.UpdateSessionList();

    }

    public async void OnPlayerListChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            RelayManager.Instance.GetPlayerList().Clear();
            foreach (var child in e.Snapshot.Children)
            {
                try
                {
                    string playerName = child.Child("PlayerName").Value?.ToString() ?? "";
                    bool isReady = bool.TryParse(child.Child("IsReady").Value?.ToString(), out bool parsedPrivate) && parsedPrivate;
                    int currentPlayers = int.Parse(child.Child("CurrentPlayers").Value?.ToString() ?? "0");
                    RelayManager.Instance.GetPlayerList().Add(new PlayerData(isReady, playerName, currentPlayers));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Firebase 데이터 처리 중 오류 발생: {ex.Message}");
                }
            }
        }
        else
        {
            RelayManager.Instance.GetPlayerList().Clear();
        }

        bool result = await UIManager.Instance.UpdatePlayerPanels();
    }

}

