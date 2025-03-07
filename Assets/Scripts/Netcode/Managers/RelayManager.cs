using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;


public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    [SerializeField] private const int MaxConnections = 4;


    private DatabaseReference dbReference;
    private List<SessionData> sessionList = new List<SessionData>();

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

    private async void Start()
    {
        await UnityServices.InitializeAsync();


        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        FirebaseDatabase.DefaultInstance.GetReference("sessions").ValueChanged += OnSessionListChanged;
    }

    private void OnSessionListChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            sessionList.Clear();
            foreach (var child in e.Snapshot.Children)
            {
                try
                {
                    string sessionName = child.Child("SessionName").Value?.ToString() ?? "Unknown";
                    string joinCode = child.Child("JoinCode").Value?.ToString() ?? "";
                    bool isPrivate = bool.TryParse(child.Child("IsPrivate").Value?.ToString(), out bool parsedPrivate) && parsedPrivate;
                    string password = child.Child("Password").Value?.ToString() ?? "";

                    sessionList.Add(new SessionData(sessionName, joinCode, isPrivate, password));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Firebase 데이터 처리 중 오류 발생: {ex.Message}");
                }
            }
        }
        else
        {
            
            sessionList.Clear();
        }

        UIManager.Instance.UpdateSessionList();
    }

    public async Task<string> CreateRelay(string sessionName, bool isPrivate, string password = "")
    {
        try
        {
            // Relay 세션 생성
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Relay created. Join code: {joinCode}");

            // NGO와 Relay 연결
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            string sessionId = dbReference.Child("sessions").Push().Key;
            SessionData newSession = new SessionData(sessionName, joinCode, isPrivate, password);
            dbReference.Child("sessions").Child(sessionId).SetRawJsonValueAsync(JsonUtility.ToJson(newSession));

            return joinCode;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create Relay: {e.Message}");
            return null;
        }
    }

    public async Task<bool> JoinRelay(string joinCode, string inputPassword = "")
    {
        try
        {
            SessionData session = sessionList.Find(s => s.JoinCode == joinCode);
            if (session == null)
            {
                Debug.LogError("세션을 찾을 수 없습니다!");
                return false;
            }
            if (session.IsPrivate && session.Password != inputPassword)
            {
                Debug.LogError("비밀번호가 틀렸습니다!");
                return false;
            }

            // Relay 세션에 연결
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            Debug.Log($"Joining relay with code: {joinCode}");

            // NGO와 Relay 연결
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                 (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join Relay: {e.Message}");
            return false;
        }

    }


    public List<SessionData> GetSessionList()
    {
        return sessionList;
    }

    public async void RemoveSessionFromFirebase(string joinCode)
    {
        try
        {
            // Firebase에서 특정 JoinCode를 가진 세션 찾기
            DataSnapshot snapshot = await dbReference.Child("sessions").GetValueAsync();

            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    string sessionJoinCode = child.Child("JoinCode").Value?.ToString();
                    if (sessionJoinCode == joinCode)
                    {
                        await dbReference.Child("sessions").Child(child.Key).RemoveValueAsync();
                        Debug.Log($"Firebase에서 세션 {joinCode} 삭제 완료");
                        return;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase에서 세션 삭제 실패: {e.Message}");
        }
    }

    [System.Serializable]
    public class SessionData
    {
        public string SessionName;
        public string JoinCode;
        public bool IsPrivate;
        public string Password;
        public int CurrentPlayers; // 현재 접속 인원
        public int MaxPlayers; // 최대 접속 가능 인원

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
}