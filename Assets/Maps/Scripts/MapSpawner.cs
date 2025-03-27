using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSpawner : MonoBehaviour
{
    public GameObject baseMapPrefab; // BaseMap 프리팹
    public int mapCount = 4; // 시작 맵 + 이어지는 3개
    public Vector3 mapOffset = new Vector3(30, 0, 0); // 맵 간 간격

    private void Start()
    {
        for (int i = 0; i < mapCount; i++)
        {
            Vector3 spawnPos = i * mapOffset;
            GameObject mapInstance = Instantiate(baseMapPrefab, spawnPos, Quaternion.identity);

            Tilemap map = mapInstance.transform.Find("Map")?.GetComponent<Tilemap>();
            BoundsInt bounds = map.cellBounds;

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase tile = map.GetTile(pos);
                if (tile is TileMapTelepoter oldTile)
                {
                    if (!oldTile.isEntrance && oldTile.teleportID == "NONE")
                    {
                        // 타일 복사 후 값 수정
                        TileMapTelepoter newTile = ScriptableObject.CreateInstance<TileMapTelepoter>();
                        newTile.sprite = oldTile.sprite;
                        newTile.teleportID = ((char)('A' + i)).ToString();
                        newTile.isEntrance = false;
                        map.SetTile(pos, newTile);
                        Debug.Log($"타일 변경됨: {map.CellToWorld(pos) + map.cellSize / 2f}, ID: {newTile.teleportID}");
                    }
                }
            }

            // 텔레포트 ID 자동 설정
            TeleportManager tpEntress = mapInstance.transform.Find("TpEntress")?.GetComponent<TeleportManager>();
            if (tpEntress != null)
            {
                tpEntress.teleportID = ((char)('A' + i + 1)).ToString(); // A, B, C, D...
            }
        }
    }
}
