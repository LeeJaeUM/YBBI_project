using Unity.Netcode;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class HostMigrationManager : TestBase
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; // 서버가 아니면 실행 안 함

        // 현재 호스트가 종료된 경우
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SelectNewHost();
        }
    }

    private void SelectNewHost()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 0)
        {
            Debug.Log("No clients available to become the new host.");
            return;
        }

        // 가장 낮은 ClientId를 가진 클라이언트를 새 호스트로 지정
        ulong newHostId = NetworkManager.Singleton.ConnectedClientsList
            .Select(client => client.ClientId)
            .OrderBy(id => id)
            .First();

        Debug.Log($"New host selected: {newHostId}");

        // 새로운 호스트에게 게임 상태를 넘겨줌
        TransferHostToClient(newHostId);
    }

    private void TransferHostToClient(ulong newHostId)
    {
        // 새로운 호스트를 클라이언트에서 서버로 승격하는 기능은 Unity NGO에 기본적으로 없음.
        // 이를 구현하려면 로컬에서 재접속하는 방식으로 처리해야 함.

        Debug.Log("Host migration is not directly supported. New host must restart as the host.");
        // 새로운 클라이언트가 `NetworkManager.Singleton.StartHost()`를 실행하도록 안내
    }

    public override void Test1(InputAction.CallbackContext context)
    {
        SelectNewHost();
    }
}
