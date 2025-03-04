using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    [SerializeField] private const int MaxConnections = 4;

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
/*
        // NetworkManager.Singleton이 null인지 확인
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton이 초기화되지 않았습니다.");
            return;
        }

        // UnityTransport가 NetworkManager에 추가되었는지 확인
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("NetworkManager에 UnityTransport 컴포넌트가 없습니다.");
            return;
        }
*/
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    /*
    /// <summary>
    /// 호스트가 Relay 서버 생성
    /// </summary>
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4); // 최대 4명 접속 가능
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Relay Join Code: {joinCode}");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            
            transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartHost(); // 호스트 시작
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    /// <summary>
    /// 클라이언트가 Relay 서버에 접속
    /// </summary>
    public async Task JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            NetworkManager.Singleton.StartClient(); // 클라이언트 시작
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
    */

    public async Task<string> CreateRelay()
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
            return joinCode;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create Relay: {e.Message}");
            return null;
        }
    }

    public async Task JoinRelay(string joinCode)
    {
        try
        {
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
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join Relay: {e.Message}");
        }
    }

}