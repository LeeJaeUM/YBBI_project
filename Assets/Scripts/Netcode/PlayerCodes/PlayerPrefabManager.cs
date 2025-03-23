using Unity.Netcode;
using UnityEngine;

public class PlayerPrefabManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs; // Warrior, Mage 등

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // 서버(호스트)만 스폰 처리

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayerForClient(clientId);
        }
    }

    private async void SpawnPlayerForClient(ulong clientId)
    {
        string joinCode = UIManager.Instance.GetSavedJoinCode(); // 저장해둔 값
        int playerIndex = int.Parse(clientId.ToString()); // 이 함수는 따로 구현 필요

        int jobIndex = await NetcodeFireBaseManager.Instance.GetSessionPlayerJobIndex(joinCode, playerIndex) - 1;

        Vector3 spawnPos = Vector3.zero; // 플레이어 별 위치 지정
        GameObject player = Instantiate(playerPrefabs[jobIndex], spawnPos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}