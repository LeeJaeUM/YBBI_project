using Unity.Netcode;
using UnityEngine;

public class PlayerJobPrefabManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs; // Warrior, Mage 등
    [Header("스폰될 맵 그리드")]
    [SerializeField] private GameObject mapGrid;
    private static bool hasSpawnedPlayers = false;
    public static bool isSessionHost = false;

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
        Debug.Log("스폰로직 실행");
        if (NetworkManager.Singleton.LocalClientId != OwnerClientId) return; // **나 자신만 조작**
        if (IsHost) isSessionHost = true;

        // 각자 자신의 플레이어 생성
        Debug.Log("각자 스폰 실행");
        SpawnMyPlayer();
    }

    private async void SpawnMyPlayer()
    {
        string joinCode = LobbyAndSesssionUIManager.Instance.GetSavedJoinCode();
        int playerIndex = LobbyAndSesssionUIManager.Instance.GetOwnPlayerIndex();

        int jobIndex = await LobbyAndSesssionFireBaseManager.Instance.GetSessionPlayerJobIndex(joinCode, playerIndex) - 1;

        MapRandomSpawner grid = mapGrid.GetComponent<MapRandomSpawner>();
        Vector3 spawnPos = grid.GridToWorld(grid.GetMapListGridCenter());

        GameObject player = Instantiate(playerPrefabs[jobIndex], spawnPos, Quaternion.identity);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
    }

    public bool GetIsSessionHost()
    {
        return isSessionHost;
    }
}