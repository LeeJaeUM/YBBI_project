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
    }




    public DatabaseReference GetDBreference()
    {
        return dbReference;
    }
    public int GetMaxConnection()
    {
        return MaxConnections;
    }

    public async Task SetCurrentPlayer(int currentNum, string joinCode)
    {
        // 🔥 세션 ID 가져오기
        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError($"세션 ID를 찾을 수 없습니다. JoinCode: {joinCode}");
            return;
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

    /*    public void ToggleReadyStatus(string joinCode, string playerID)
        {
            SessionData session = sessionList.Find(s => s.JoinCode == joinCode);
            string sessionId = session.JoinCode;
            string playerPath = $"sessions/{sessionId}/players/{playerID}/IsReady";

            dbReference.Child(playerPath).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    bool currentStatus = bool.Parse(task.Result.Value.ToString());
                    dbReference.Child(playerPath).SetValueAsync(!currentStatus);
                }
            });
        }
    */


    public async void AddPlayer(string joinCode)
    {
        Debug.Log("플레이어 추가 시도");

        int currentPlayers = await GetCurrentPlayer(joinCode);
        if (currentPlayers >= MaxConnections)
        {
            Debug.Log("이미 정원입니다.");
            return;
        }

        string sessionId = await GetSessionIdByJoinCode(joinCode);
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogError("세션 ID를 찾을 수 없습니다.");
            return;
        }

        PlayerData newPlayer = new PlayerData(false, currentPlayers.ToString());
        UIManager.Instance.SetSavedPlayerID(currentPlayers.ToString());

        await dbReference.Child("sessions").Child(sessionId).Child("Players").Child(currentPlayers.ToString()).SetValueAsync(newPlayer.ToDictionary());
        await SetCurrentPlayer(1, joinCode);

        Debug.Log($"세션 {sessionId}에 {currentPlayers}번째 플레이어 추가 완료!");
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
            newList.Add(new PlayerData(false, i.ToString()));
        }

        return newList;
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

    public void RequestRemovePlayer()
    {

    }


    public void OnSessionListChanged(object sender, ValueChangedEventArgs e)
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
}

