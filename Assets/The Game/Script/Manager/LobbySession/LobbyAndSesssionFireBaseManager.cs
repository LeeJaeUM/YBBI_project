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
using UnityEngine.InputSystem.HID;

public class LobbyAndSesssionFireBaseManager : MonoBehaviour
{
    [SerializeField] private const int MaxConnections = 3;

    public static LobbyAndSesssionFireBaseManager Instance;
    private DatabaseReference dbReference;

    public int _currentPlayers = 0;

    private string firebaseDatabaseUrl = "https://unityybbi-default-rtdb.firebaseio.com/";

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
    }


    public void AddDefaultInstancePlayerList(string sessionId)
    {
        FirebaseDatabase.DefaultInstance.GetReference("sessions").Child(sessionId).Child("Players").ValueChanged += OnPlayerListChanged;
    }
    public void RemoveDefaultInstancePlayerList(string sessionId)
    {
        FirebaseDatabase.DefaultInstance.GetReference("sessions").Child(sessionId).Child("Players").ValueChanged -= OnPlayerListChanged;
        Debug.Log($"{sessionId}에서의 인스턴스 제거 완료");
    }

    public DatabaseReference GetDBreference()
    {
        return dbReference;
    }
    public int GetMaxConnection()
    {
        return MaxConnections;
    }

    public void AddFireBaseSession(string sessionId, FireBaseSessionData newSession)
    {
        dbReference.Child("sessions").Child(sessionId).SetRawJsonValueAsync(JsonUtility.ToJson(newSession));
    }

    public async void StartSessionSetting(string sessionId)
    {
        for (int i = 0; i < MaxConnections; i++)
        {
            PlayerData emptyPlayer = new PlayerData(true, null, 0);
            await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(i.ToString()).SetValueAsync(emptyPlayer.ToDictionary());
        }
    }

    public List<PlayerData> newPlayerListMaker()
    {
        List<PlayerData> newList = new List<PlayerData>();
        for (int i = 0; i < MaxConnections ; i++)
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

        string playerName = TheGameAuthManager.Instance.GetPlayerNickName();
        Debug.Log($"player이름 : {playerName}");
        PlayerData newPlayer = new PlayerData(false, playerName, 1); //임시로 플레이어 네임을 아무렇게나 넣엇음
        LobbyAndSesssionUIManager.Instance.SetOwnPlayerIndex(playerIndex);

        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).SetValueAsync(newPlayer.ToDictionary());
        await SetCurrentPlayer(1, joinCode);

        Debug.Log($"세션 {sessionId}에 {playerIndex}번째 플레이어 추가 완료!");

        AddDefaultInstancePlayerList(sessionId);


        return true;
    }



    public async Task<bool> RemovePlayerFromSession(string joinCode, int playerIndex)
    {
        Debug.Log($"{playerIndex}번째 플레이어 제거 시도");

        // JoinCode로 SessionID 찾기
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("세션을 찾을 수 없습니다.");
            return false;
        }

        Debug.Log($"제거할 플레이어가 있는 세션 ID: {sessionId}");

        if(playerIndex == -1)
        {
            return false;
        }

        // 플레이어 데이터 제거
        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).RemoveValueAsync();

        PlayerData emptyPlayer = new PlayerData(true, null, 0);
        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).SetValueAsync(emptyPlayer.ToDictionary());

        Debug.Log($"플레이어 {playerIndex} 제거 완료");

        RemoveDefaultInstancePlayerList(sessionId);
        // 현재 플레이어 수 업데이트

        LobbyAndSesssionUIManager.Instance.SetOwnPlayerIndex(-1);
        await SetCurrentPlayer(-1, joinCode);
        await LobbyAndSesssionUIManager.Instance.UpdatePlayerPanels();

        return true;
    }
    public async void RequseReversalReadyToggle(string joincode, string playerName)
    {
        await ReversalReadyToggle(joincode, playerName);
    }

    private async Task<bool> ReversalReadyToggle(string joinCode, string playerName)
    {
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        // Players 경로에서 모든 데이터 가져오기
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("Players").GetValueAsync();

        int playerIndex = LobbyAndSesssionUIManager.Instance.GetOwnPlayerIndex();

        if (playerIndex == -1)
        {
            Debug.Log("플레이어 index 가져오기 실패");
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

    public async Task<bool> IsAllPlayerReady(string joinCode)
    {
        for (int i = 1;i< MaxConnections;i++)
        {
            bool isReady = await GetSessionPlayerIsReady(joinCode, i);
            if(!isReady)
            {
                Debug.Log($"{i}번째 플레이어 준비 안됨");
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

    public async void SetIsStartInFireBase(string joinCode, bool isStart)
    {
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            return;
        }
        await dbReference.Child("sessions").Child(sessionId).Child("IsStart").SetValueAsync(isStart);
        Debug.Log("파이어 베이스 시작상태 변경 성공");
    }

    public async Task<bool> GetIsStartInFireBase(string joinCode)
    {
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            return true;
        }
        DataSnapshot snapshot = await dbReference.Child("sessions").Child(sessionId).Child("IsStart").GetValueAsync();

        bool isStart = false;
        if(snapshot.Value.ToString() == true.ToString())
        {
            isStart = true;
        }

        Debug.Log("파이어 베이스 시작상태 return 성공");
        return isStart;
    }


    public async Task<string> GetSessionPlayerName(string joinCode, int playerIndex)
    {
        Debug.Log("플레이어 이름 가져오기 시도");
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if(string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("플레이어 이름 가져오기 에서 세션 아이디 찾기 실패");
            return "";
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

    public async void SetSessionPlayerJobIndex(string joinCode, int playerIndex, int jobIndex)
    {
        Debug.Log("플레이어 직업 인덱스 설정 시도");
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("플레이어 직업 인덱스 설정 에서 세션 아이디 찾기 실패");
            return;
        }
        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(playerIndex.ToString()).Child("PlayerJobIndex").SetValueAsync(jobIndex);
        Debug.Log("플레이어 직어 인덱스 설정 완료");
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
            var sessionId = await GetSessionIdByJoinCode(joinCode);
            if (sessionId != null)
            {
                await dbReference.Child("sessions").Child(sessionId).RemoveValueAsync();
                Debug.Log("파이어베이스 세션 제거성공");
                RemoveDefaultInstancePlayerList(sessionId);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase에서 세션 삭제 실패: {e.Message}");
        }

        LobbyAndSesssionUIManager.Instance.SetOwnPlayerIndex(-1);
    }

    public async void OnSessionListChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            GameRelayManager.Instance.GetSessionList().Clear();
            foreach (var child in e.Snapshot.Children)
            {
                try
                {
                    bool isStart = bool.TryParse(child.Child("IsStart").Value?.ToString(), out bool parsedPrivate_0) && parsedPrivate_0;
                    string sessionName = child.Child("SessionName").Value?.ToString() ?? "Unknown";
                    string joinCode = child.Child("JoinCode").Value?.ToString() ?? "";
                    bool isPrivate = bool.TryParse(child.Child("IsPrivate").Value?.ToString(), out bool parsedPrivate_1) && parsedPrivate_1;
                    string password = child.Child("Password").Value?.ToString() ?? "";
                    int currentPlayers = int.Parse(child.Child("CurrentPlayers").Value?.ToString() ?? "0"); 
                    List<PlayerData> Players = new List<PlayerData>();

                    GameRelayManager.Instance.GetSessionList().Add(new FireBaseSessionData(isStart, sessionName, joinCode, isPrivate, password, currentPlayers, Players));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Firebase 데이터 처리 중 오류 발생: {ex.Message}");
                }
            }
        }
        else
        {
            GameRelayManager.Instance.GetSessionList().Clear();
        }

       
        LobbyAndSesssionUIManager.Instance.UpdateSessionList();

    }

    public async void OnPlayerListChanged(object sender, ValueChangedEventArgs e)
    {
        if (!e.Snapshot.Exists)
        {
            Debug.Log("Players 경로가 비어 있습니다. 모든 패널 초기화.");
            LobbyAndSesssionUIManager.Instance.ResetAllPlayerPanels();  // 패널 전체 초기화
            return;
        }

        for (int i = 0; i < MaxConnections; i++)
        {
            var playerSnapshot = e.Snapshot.Child(i.ToString());
            if (playerSnapshot.Exists)
            {
                try
                {
                    string playerName = playerSnapshot.Child("PlayerName").Value?.ToString() ?? "";
                    bool isReady = bool.TryParse(playerSnapshot.Child("IsReady").Value?.ToString(), out bool parsedReady) && parsedReady;
                    int playerJobIndex = int.TryParse(playerSnapshot.Child("PlayerJobIndex").Value?.ToString(), out int parsedJob) ? parsedJob : 0;
                    
                    if (string.IsNullOrEmpty(playerName))
                    {
                        LobbyAndSesssionUIManager.Instance.ResetSinglePlayerPanel(i);
                        Debug.Log($"[Player {i}] 플레이어 이름 없음 → 패널 초기화");
                        continue;
                    }

                    LobbyAndSesssionUIManager.Instance.UpdateSinglePlayerPanel(i, playerName, isReady, playerJobIndex, i == 0);
                    Debug.Log($"[Player {i}] 업데이트됨: {playerName}, Ready: {isReady}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Player {i}] 처리 중 오류: {ex.Message}");
                }
            }
            else
            {
                // 플레이어가 없을 경우 패널 초기화
                LobbyAndSesssionUIManager.Instance.ResetSinglePlayerPanel(i);
                Debug.Log($"[Player {i}] 없음 → 패널 초기화");
            }
        }
    }

}

