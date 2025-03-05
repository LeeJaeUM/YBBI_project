using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Collections.Generic;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    [SerializeField] private const int MaxConnections = 4;

    private List<SessionData> sessionList = new List<SessionData>();

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

            sessionList.Add(new SessionData(sessionName, joinCode, isPrivate, password));

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

    [System.Serializable]
    public class SessionData
    {
        public string SessionName;
        public string JoinCode;
        public bool IsPrivate;
        public string Password;

        public SessionData(string sessionName, string joinCode, bool isPrivate, string password)
        {
            SessionName = sessionName;
            JoinCode = joinCode;
            IsPrivate = isPrivate;
            Password = password;
        }
    }
}