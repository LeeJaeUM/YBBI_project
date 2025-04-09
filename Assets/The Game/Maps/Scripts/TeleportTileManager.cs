using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportTileManager : MonoBehaviour
{
    public string teleportID; // 이 입구가 연결할 출구 ID

    public bool isTp = false;

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if(!isTp)
        {
            
            Debug.Log($"[텔레포트] 요청 ID: {teleportID}");

            // 현재 맵 (Grid) 참조
            GameObject currentMap = GetComponentInParent<Grid>().gameObject;

            // 모든 TeleportManager 찾기
            TeleportTileManager[] allTeleporters = FindObjectsOfType<TeleportTileManager>();

            foreach (TeleportTileManager targetTeleporter in allTeleporters)
            {
                // 자기 자신 무시
                if (targetTeleporter == this)
                    continue;

                // 같은 맵(Grid 내부)에 있는 포탈 무시
                if (targetTeleporter.transform.IsChildOf(currentMap.transform))
                    continue;

                // teleportID 같은 포탈 발견 시
                if (targetTeleporter.teleportID == this.teleportID)
                {
                    // 해당 포탈이 속한 타일맵 찾기
                    Tilemap targetTilemap = targetTeleporter.GetComponent<Tilemap>();
                    if (targetTilemap != null)
                    {
                        // 실제 존재하는 타일 위치만 가져오기
                        foreach (Vector3Int pos in targetTilemap.cellBounds.allPositionsWithin)
                        {
                            if (!targetTilemap.HasTile(pos))
                                continue;  // 빈 타일 무시

                            Vector3 teleportPosition = targetTilemap.GetCellCenterWorld(pos);
                            PlayerPosRPC posRpc = collision.GetComponent<PlayerPosRPC>();
                            if (posRpc != null)
                            {
                                posRpc.TeleportRequest(teleportPosition);
                                isTp = true;
                            }
                            return;
                        }
                    }
                }
            }
        }
        isTp = false;
    }
}
