using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportTileManager : MonoBehaviour
{
    public string teleportID; // 이 입구가 연결할 출구 ID

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

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
                Vector3 teleportPosition;

                if (targetTilemap != null)
                {
                    Debug.Log("타겟 위치 존재함");
                    // 타일맵이 존재하면, 정확한 셀 위치 중앙에 스폰
                    Vector3Int cellPosition = targetTilemap.WorldToCell(targetTeleporter.transform.position);
                    teleportPosition = targetTilemap.GetCellCenterWorld(cellPosition);
                }
                else
                {
                    // 타일맵이 없다면, 포탈 오브젝트 위치에 스폰
                    teleportPosition = targetTeleporter.transform.position;
                }


                // 플레이어를 찾은 위치로 이동
                PlayerPosRPC posRpc = collision.GetComponent<PlayerPosRPC>();
                if (posRpc != null)
                {
                    posRpc.TeleportRequest(teleportPosition);
                }
                return;
            }
        }

        Debug.LogWarning($"다른 맵에서 teleportID '{teleportID}'를 가진 포탈을 찾지 못했습니다!");
    }
}
