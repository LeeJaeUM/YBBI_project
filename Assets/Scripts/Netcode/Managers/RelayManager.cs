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

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    private List<SessionData> sessionList = new List<SessionData>();
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
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public async Task<string> CreateRelay(string sessionName, bool isPrivate, string password = "")
    {
        try
        {

            // Relay 세션 생성
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(NetcodeFireBaseManager.Instance.GetMaxConnection());
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

            string sessionId = NetcodeFireBaseManager.Instance.GetDBreference().Child("sessions").Push().Key;
            SessionData newSession = new SessionData(false ,sessionName, joinCode, isPrivate, password, 0, NetcodeFireBaseManager.Instance.newPlayerListMaker());

            NetcodeFireBaseManager.Instance.AddFireBaseSession(sessionId, newSession);
            NetcodeFireBaseManager.Instance.StartSessionSetting(sessionId);

            AddInstanceDissconection();

            NetcodeFireBaseManager.Instance.AddPlayer(joinCode);

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
            NetcodeFireBaseManager.Instance.AddPlayer(joinCode);

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

        if(!NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트와의 연결이 끊겼으므로 세션 리스트 UI로 이동");

            // UIManager를 이용해 세션 리스트 화면으로 이동
            UIManager.Instance.HideCreateSessionUI();

            // 네트워크 정리
            NetworkManager.Singleton.Shutdown();

            
        }


        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public List<SessionData> GetSessionList()
    {
        return sessionList;
    }
    public List<PlayerData> GetPlayerList()
    {
        return playerList;
    }
}