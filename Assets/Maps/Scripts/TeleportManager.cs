using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportManager : MonoBehaviour
{
    public string teleportID; // 이 입구가 연결할 출구 ID

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Debug.Log($"[텔레포트] 요청 ID: {teleportID}");

        
        Tilemap[] allMaps = FindObjectsOfType<Tilemap>(); // 씬 전체에서 Map 타일맵을 가진 오브젝트들을 찾음

        foreach (Tilemap map in allMaps)
        {
            if (map.gameObject.name != "Map") continue; // Map 이름의 타일맵만

            Vector3 exitPos = FindMatchingExit(map, teleportID);
            if (exitPos != Vector3.zero)
            {
                NetCodePlayerHandler handler = collision.GetComponent<NetCodePlayerHandler>();
                if (handler != null)
                {
                    handler.TeleportRequest(exitPos);
                }
                return;
            }
        }

        Debug.LogWarning($"출구 '{teleportID}' 를 가진 타일을 어떤 맵에서도 못 찾음!");
    }

    Vector3 FindMatchingExit(Tilemap map, string id)
    {
        BoundsInt bounds = map.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = map.GetTile(pos);
            if (tile is TileMapTelepoter targetTile)
            {
                if (!targetTile.isEntrance && targetTile.teleportID == id)
                {
                    Debug.Log($"[텔레포트] 출구 위치 찾음: {map.name} - {pos}");
                    return map.CellToWorld(pos) + map.cellSize / 2f;
                }
            }
        }
        return Vector3.zero;
    }
}
