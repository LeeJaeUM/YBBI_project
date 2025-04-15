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
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using Unity.Multiplayer.Playmode;
using UnityEditor;
using Firebase.Extensions;

public class GameRelayManager : MonoBehaviour
{
    public static GameRelayManager Instance;
    private List<FireBaseSessionData> sessionList = new List<FireBaseSessionData>();
    private List<PlayerData> playerList = new List<PlayerData>();

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

    private async void Start()
    {
        await UnityServices.InitializeAsync();


        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }

    public void AddInstanceDissconection()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public async Task<string> CreateRelay(string sessionName, bool isPrivate, string password = "") 
    {
        try
        {
            // Relay 세션 생성
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(LobbyAndSesssionFireBaseManager.Instance.GetMaxConnection());
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

            string sessionId = LobbyAndSesssionFireBaseManager.Instance.GetDBreference().Child("sessions").Push().Key;
            FireBaseSessionData newSession = new FireBaseSessionData(false ,sessionName, joinCode, isPrivate, password, 0, LobbyAndSesssionFireBaseManager.Instance.newPlayerListMaker());

            LobbyAndSesssionFireBaseManager.Instance.AddFireBaseSession(sessionId, newSession);
            LobbyAndSesssionFireBaseManager.Instance.StartSessionSetting(sessionId);

            AddInstanceDissconection();

            LobbyAndSesssionFireBaseManager.Instance.AddPlayer(joinCode);

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
            FireBaseSessionData session = sessionList.Find(s => s.JoinCode == joinCode);
            if (session == null)
            {
                Debug.Log("세션을 찾을 수 없습니다!");
                return false;
            }
            if (session.IsPrivate && session.Password != inputPassword)
            {
                Debug.Log("비밀번호가 틀렸습니다!");
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
            LobbyAndSesssionFireBaseManager.Instance.AddPlayer(joinCode);

            AddInstanceDissconection();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join Relay: {e.Message}");
            return false;
        }
    }

    
    public ulong GetClientID()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

    

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"클라이언트 {clientId} 연결 끊김 감지됨");

        // 호스트인 경우: 클라이언트가 나간 것에 대해 아무 처리도 하지 않음
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트는 클라이언트의 연결 해제를 감지했지만 무시합니다.");
            return;
        }

        // 클라이언트가 자기 자신의 연결 해제를 감지한 경우만 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("자기 자신의 연결 해제 감지 → UI 초기화 및 종료");

            // UI 초기화
            if (LobbyAndSesssionUIManager.Instance != null)
            {
                LobbyAndSesssionUIManager.Instance.HideCreateSessionUI();
            }

            // 네트워크 종료
            NetworkManager.Singleton.Shutdown();
        }

        // 이벤트 중복 방지
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public List<FireBaseSessionData> GetSessionList()
    {
        return sessionList;
    }
    public List<PlayerData> GetPlayerList()
    {
        return playerList;
    }
}