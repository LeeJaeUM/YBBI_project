using Unity.Netcode;
using UnityEngine;

public class PlayerJobPrefabManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs; // Warrior, Mage 등
    [Header("스폰될 맵 그리드")]
    [SerializeField] private GameObject mapGrid;
    private static bool hasSpawnedPlayers = false;

    private Vector3 _pos;

    private void Awake()
    {
        if (hasSpawnedPlayers) return;

        MapRandomSpawner grid = mapGrid.GetComponent<MapRandomSpawner>();


        _pos = grid.GridToWorld(grid.GetMapListGridCenter());

        hasSpawnedPlayers = true;
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // 서버(호스트)만 스폰 처리

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"플레이어ID : {clientId}스폰성공");
            SpawnPlayerForClient(clientId);
        }
    }

    private async void SpawnPlayerForClient(ulong clientId)
    {
        string joinCode = LobbyAndSesssionUIManager.Instance.GetSavedJoinCode(); // 저장해둔 값
        int playerIndex = LobbyAndSesssionUIManager.Instance.GetOwnPlayerIndex();

        int jobIndex = await LobbyAndSesssionFireBaseManager.Instance.GetSessionPlayerJobIndex(joinCode, playerIndex) - 1;

        Vector3 spawnPos = _pos; // 플레이어 별 위치 지정
        GameObject player = Instantiate(playerPrefabs[jobIndex], spawnPos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}