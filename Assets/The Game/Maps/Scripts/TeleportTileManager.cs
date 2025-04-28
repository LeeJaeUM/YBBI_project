using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TeleportTileManager : MonoBehaviour
{
    public string teleportID;
    // 텔레포트 ID

    [SerializeField] private GameObject mapOBJ;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (teleportID == null) return;


        PlayerPosRPC posRpc = collision.GetComponent<PlayerPosRPC>();
        if (posRpc == null || posRpc.IsTeleporting()) // 중복 방지
            return;

        // 모든 TeleportManager 찾기
        TeleportTileManager[] allTeleporters = FindObjectsOfType<TeleportTileManager>(true);

        foreach (TeleportTileManager targetTeleporter in allTeleporters)
        {
            if (targetTeleporter == this) continue;
            if (targetTeleporter.transform.IsChildOf(mapOBJ.transform)) continue;
            if (targetTeleporter.teleportID != teleportID) continue;

            MapData targetMapData = targetTeleporter.mapOBJ.GetComponent<MapData>();
            if (targetMapData == null) continue;

            bool isConnected = false;

            if (targetMapData.tpUp == targetTeleporter.gameObject && targetMapData.isTpUpSeted)
                isConnected = true;
            if (targetMapData.tpDown == targetTeleporter.gameObject && targetMapData.isTpDownSeted)
                isConnected = true;
            if (targetMapData.tpLeft == targetTeleporter.gameObject && targetMapData.isTpLeftSeted)
                isConnected = true;
            if (targetMapData.tpRight == targetTeleporter.gameObject && targetMapData.isTpRightSeted)
                isConnected = true;

            if (!isConnected) continue; // 연결 안 된 포탈이면 스킵

            // 해당 포탈이 속한 타일맵 찾기
            Tilemap targetTilemap = targetTeleporter.GetComponent<Tilemap>();
            if (targetTilemap == null) continue;

            // 실제 존재하는 타일 위치만 가져오기
            foreach (Vector3Int pos in targetTilemap.cellBounds.allPositionsWithin)
            {
                if (!targetTilemap.HasTile(pos)) continue;

                Vector3 teleportPosition = targetTilemap.GetCellCenterWorld(pos);
                posRpc.TeleportRequest(teleportPosition);
                Debug.Log($"[텔레포트] {teleportID} → {teleportPosition}");
                return;
            }
        }
    }

    private void Awake()
    {
    }
}
