using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TeleportTileManager : NetworkBehaviour
{
    public string teleportID; // 텔레포트 ID

    [SerializeField] private GameObject mapOBJ;

    private MapData mapData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (teleportID == null) return;

        var playerNetObj = collision.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        // 클라이언트든 호스트든 서버에 요청
        RequestTeleportServerRpc(playerNetObj.OwnerClientId, teleportID);
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestTeleportServerRpc(ulong clientId, string teleportID)
    {
        // 요청한 플레이어를 찾아 이동
        NetworkObject netObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (netObj == null) return;

        var posRpc = netObj.GetComponent<PlayerPosRPC>();
        if (posRpc == null || posRpc.IsTeleporting()) return;

        // 목적지 계산 (호스트 기준)
        TeleportTileManager[] allTeleporters = FindObjectsOfType<TeleportTileManager>(true);
        foreach (TeleportTileManager tp in allTeleporters)
        {
            if (tp == this) continue;
            if (tp.teleportID != teleportID) continue;
            if (tp.transform.IsChildOf(mapOBJ.transform)) continue;

            Tilemap tilemap = tp.GetComponent<Tilemap>();
            if (tilemap == null) continue;

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos)) continue;

                Vector3 teleportPosition = tilemap.GetCellCenterWorld(pos);
                Debug.Log($"[서버 텔레포트] Client {clientId} → {teleportPosition}");

                MapData targetData = tilemap.GetComponentInParent<MapData>();
                if (targetData != null &&
                    (targetData.roomType == Enums.RoomType.Enemy || targetData.roomType == Enums.RoomType.Boss))
                {
                    var enemySpawner = targetData.GetComponentInChildren<EnemySpawnManager>();
                    enemySpawner.RequestEnterFight();
                }

                posRpc.TeleportRequest(teleportPosition);
                return;
            }
        }
    }

    private void Awake()
    {
        mapData = mapOBJ.GetComponent<MapData>();
    }
}
