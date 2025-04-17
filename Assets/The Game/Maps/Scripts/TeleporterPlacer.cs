using UnityEngine.Tilemaps;
using UnityEngine;

public class TeleporterPlacer : MonoBehaviour
{
    public TelepoterTile teleporterTileAsset;
    public string teleportIdPrefix = "TP";
    private int idCounter = 0;

    public void ConnectRooms(GameObject roomA, GameObject roomB, Vector2Int directionFromAtoB)
    {
        string teleportID = teleportIdPrefix + ((char)('A' + idCounter++)); // 고유 ID

        // A → B 방향으로 포탈 생성
        PlacePortal(roomA, directionFromAtoB, teleportID);       // 예: 오른쪽
        PlacePortal(roomB, -directionFromAtoB, teleportID);      // 예: 왼쪽

        Debug.Log($"포탈 연결: {roomA.name} → {roomB.name} via {directionFromAtoB}, teleportID = {teleportID}");
    }

    private void PlacePortal(GameObject roomObj, Vector2Int direction, string teleportID)
    {
        Transform tilemapObj = roomObj.transform.Find("TeleportMap");
        if (tilemapObj == null)
        {
            Debug.LogWarning($"TeleportMap 없음: {roomObj.name}");
            return;
        }

        Tilemap map = tilemapObj.GetComponent<Tilemap>();
        if (map == null) return;

        Vector3Int cell = GetPortalCellByDirection(direction);

        var tile = ScriptableObject.CreateInstance<TelepoterTile>();
        tile.teleportID = teleportID;
        tile.sprite = teleporterTileAsset.sprite;
        tile.color = teleporterTileAsset.color;

        map.SetTile(cell, tile);
        Debug.Log($"[포탈배치] {roomObj.name} - 방향 {direction} - ID {teleportID}");
    }

    private Vector3Int GetPortalCellByDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return new Vector3Int(0, 4, 0);
        if (dir == Vector2Int.down) return new Vector3Int(0, -4, 0);
        if (dir == Vector2Int.left) return new Vector3Int(-6, 0, 0);
        if (dir == Vector2Int.right) return new Vector3Int(6, 0, 0);
        return Vector3Int.zero;
    }
}
