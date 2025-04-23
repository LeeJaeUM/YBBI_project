using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapRandomSpawner : MonoBehaviour
{
    [SerializeField] private Grid mapSpawnGrid;
    [SerializeField] private MapListSO mapPrefabList; // BaseMap 프리팹
    [SerializeField] private int mapCount = 13;
    [SerializeField] private int stageNum = 1;

    public const int MAP_SIZE = 10; //MAP_SIZE * MAP_SIZE의 크기의 맵 배열
    private MapManager[,] _mapGrid = new MapManager[MAP_SIZE, MAP_SIZE];

    Vector2Int[] _directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };


    #region 맵생성로직

    private void ReqestMapSpawn()
    {
        Vector2Int center = GetMapListGridCenter();
        MapManager startRoom = GetRandomRoom(RoomType.Start);

        GameObject startObj = Instantiate(startRoom.gameObject, GridToWorld(center), Quaternion.identity, mapSpawnGrid.transform);
        MapManager startData = startObj.GetComponent<MapManager>();
        _mapGrid[center.x, center.y] = startData;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(center);
        int placedRoomCount = 1;

        while (frontier.Count > 0 && placedRoomCount < mapCount - 1)
        {
            Vector2Int currentPos = frontier.Dequeue();
            MapManager currentRoom = _mapGrid[currentPos.x, currentPos.y];

            List<Vector2Int> dirList = new List<Vector2Int>(_directions);
            Shuffle(dirList);

            foreach (var dir in dirList)
            {
                Vector2Int nextPos = currentPos + dir;
                if (!IsValidPos(nextPos)) continue;
                if (_mapGrid[nextPos.x, nextPos.y] != null) continue;
                if (FormsSquare(nextPos)) continue;

                MapManager newRoom = GetRandomCompatibleRoom(RoomType.Normal, dir, currentRoom);
                if (newRoom == null) continue;

                GameObject newObj = Instantiate(newRoom.gameObject, GridToWorld(nextPos), Quaternion.identity, mapSpawnGrid.transform);
                MapManager newData = newObj.GetComponent<MapManager>();
                _mapGrid[nextPos.x, nextPos.y] = newData;

                frontier.Enqueue(nextPos);
                placedRoomCount++;

                if (placedRoomCount >= mapCount - 1)
                    break;
            }
        }

        // 보스방 연결 시도
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                if (_mapGrid[x, y] == null) continue;

                foreach (var dir in _directions)
                {
                    Vector2Int bossPos = new Vector2Int(x, y) + dir;
                    if (!IsValidPos(bossPos)) continue;
                    if (_mapGrid[bossPos.x, bossPos.y] != null) continue;
                    if (FormsSquare(bossPos)) continue;

                    MapManager bossRoom = GetRandomCompatibleRoom(RoomType.Boss, dir, _mapGrid[x, y]);
                    if (bossRoom != null)
                    {
                        GameObject bossObj = Instantiate(bossRoom.gameObject, GridToWorld(bossPos), Quaternion.identity, mapSpawnGrid.transform);
                        MapManager bossData = bossObj.GetComponent<MapManager>();
                        _mapGrid[bossPos.x, bossPos.y] = bossData;
                        return;
                    }
                }
            }
        }
    }


    public Vector2Int GetMapListGridCenter()
    {
        return new Vector2Int(MAP_SIZE / 2, MAP_SIZE / 2);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3 worldPos = mapSpawnGrid.CellToWorld((Vector3Int)gridPos);
        return worldPos + mapSpawnGrid.cellSize / 2f;
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < MAP_SIZE && pos.y < MAP_SIZE;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }


    private bool FormsSquare(Vector2Int pos)
    {
        Vector2Int[][] squareOffsets = new Vector2Int[][]
        {
        new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up },
        new[] { Vector2Int.zero, Vector2Int.left, Vector2Int.up, Vector2Int.left + Vector2Int.up },
        new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.down, Vector2Int.right + Vector2Int.down },
        new[] { Vector2Int.zero, Vector2Int.left, Vector2Int.down, Vector2Int.left + Vector2Int.down },
        };

        foreach (var offsets in squareOffsets)
        {
            bool allFilled = true;
            foreach (var offset in offsets)
            {
                Vector2Int checkPos = pos + offset;
                if (!IsValidPos(checkPos) || _mapGrid[checkPos.x, checkPos.y] == null)
                {
                    allFilled = false;
                    break;
                }
            }
            if (allFilled) return true;
        }

        return false;
    }

    private MapManager GetRandomRoom(RoomType type)
    {
        List<MapManager> list = mapPrefabList.Maps.FindAll(m => m.roomType == type);
        return list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
    }

    private MapManager GetRandomCompatibleRoom(RoomType type, Vector2Int fromDir, MapManager fromRoom)
    {
        List<MapManager> list;
        if (type != RoomType.Normal)
        {
            list = mapPrefabList.Maps.FindAll(m => m.roomType == RoomType.Normal || m.roomType == RoomType.Enemy);
        }
        else
        {
            list = mapPrefabList.Maps.FindAll(m => m.roomType == RoomType.Boss);
        }

        if (fromRoom == null)
            return list.PickRandom(); // fromRoom이 null이면 아무거나 리턴

        return list.FindAll(m => IsCompatible(fromDir, fromRoom, m)).PickRandom();
    }

    private bool IsCompatible(Vector2Int dir, MapManager from, MapManager to)
    {
        if (dir == Vector2Int.up) return from.canConnectUp && to.canConnectDown;
        if (dir == Vector2Int.down) return from.canConnectDown && to.canConnectUp;
        if (dir == Vector2Int.left) return from.canConnectLeft && to.canConnectRight;
        if (dir == Vector2Int.right) return from.canConnectRight && to.canConnectLeft;
        return false;
    }
    #endregion

    #region 텔레포트 연결 로직
    private void AssignTeleportIDs()
    {
        int idCounter = 0;
        MapManager BossRoom;
        Vector2Int[] teleportDirs = { Vector2Int.down, Vector2Int.left }; // 한쪽만 처리해서 중복 방지

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var currentRoom = _mapGrid[x, y];
                if (currentRoom == null) continue;
                Vector2Int currentPos = new Vector2Int(x, y);

                foreach (var dir in teleportDirs)
                {
                    Vector2Int neighborPos = currentPos + dir;
                    if (!IsValidPos(neighborPos)) continue;

                    var neighborRoom = _mapGrid[neighborPos.x, neighborPos.y];
                    if (neighborRoom == null) continue;

                    string tpID = $"TP_{stageNum}_{idCounter}";

                    if (currentRoom.roomType != RoomType.Boss || neighborRoom.roomType != RoomType.Boss)
                    {
                        if (dir == Vector2Int.down &&
                        !currentRoom.isTpDownSeted && !currentRoom.isTpDownSeted &&
                        neighborRoom.canConnectUp && !neighborRoom.isTpUpSeted)
                        {
                            SetTeleportID(currentRoom.tpDown, tpID);
                            SetTeleportID(neighborRoom.tpUp, tpID);
                            currentRoom.isTpDownSeted = true;
                            neighborRoom.isTpUpSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ↓ {currentRoom.name} ↔ ↑ {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                        if (dir == Vector2Int.left &&
                        currentRoom.canConnectLeft && !currentRoom.isTpLeftSeted &&
                        neighborRoom.canConnectRight && !neighborRoom.isTpRightSeted)
                        {
                            SetTeleportID(currentRoom.tpLeft, tpID);
                            SetTeleportID(neighborRoom.tpRight, tpID);
                            currentRoom.isTpLeftSeted = true;
                            neighborRoom.isTpRightSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ← {currentRoom.name} ↔ → {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                    }
                    else if (currentRoom.roomType == RoomType.Boss)
                    {
                        BossRoom = currentRoom;
                        if (dir == Vector2Int.down &&
                        !currentRoom.isTpDownSeted && neighborRoom.canConnectUp &&
                        !neighborRoom.isTpUpSeted)
                        {
                            SetTeleportID(currentRoom.tpDown, tpID);
                            SetTeleportID(neighborRoom.tpUp, tpID);
                            currentRoom.isTpDownSeted = true;
                            neighborRoom.isTpUpSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ↓ {currentRoom.name} ↔ ↑ {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                        if (dir == Vector2Int.left &&
                        !currentRoom.isTpDownSeted && neighborRoom.canConnectRight &&
                        !neighborRoom.isTpRightSeted)
                        {
                            SetTeleportID(currentRoom.tpDown, tpID);
                            SetTeleportID(neighborRoom.tpRight, tpID);
                            currentRoom.isTpDownSeted = true;
                            neighborRoom.isTpRightSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ← {currentRoom.name} ↔ → {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                    }
                    else if (neighborRoom.roomType == RoomType.Boss)
                    {
                        if (dir == Vector2Int.down &&
                        !currentRoom.isTpDownSeted && !currentRoom.isTpDownSeted &&
                        neighborRoom.canConnectDown && !neighborRoom.isTpDownSeted)
                        {
                            SetTeleportID(currentRoom.tpDown, tpID);
                            SetTeleportID(neighborRoom.tpUp, tpID);
                            currentRoom.isTpDownSeted = true;
                            neighborRoom.isTpDownSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ↓ {currentRoom.name} ↔ ↑ {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                        if (dir == Vector2Int.left &&
                        currentRoom.canConnectLeft && !currentRoom.isTpLeftSeted &&
                        neighborRoom.canConnectDown && !neighborRoom.isTpDownSeted)
                        {
                            SetTeleportID(currentRoom.tpLeft, tpID);
                            SetTeleportID(neighborRoom.tpUp, tpID);
                            currentRoom.isTpLeftSeted = true;
                            neighborRoom.isTpDownSeted = true;
                            Debug.Log($"{idCounter}번째 텔레포터 연결 ← {currentRoom.name} ↔ → {neighborRoom.name} @ {tpID}");
                            idCounter++;
                        }
                    }
                }
            }
        }
    }


    private void SetTeleportID(GameObject tpObj, string id)
    {
        if (tpObj == null) return;

        var manager = tpObj.GetComponent<TeleportTileManager>();
        if (manager != null)
        {
            manager.teleportID = id;
        }

        var tilemap = tpObj.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos)) continue;
                var tile = tilemap.GetTile(pos) as TileMapTelepoter;
                if (tile != null)
                {
                    tile.teleportID = id;
                    break; // 한 타일만 ID 설정하면 됨
                }
            }
        }
    }

    #endregion

    #region 맵초기화 로직
    private void ResetAllMapPrefabs()
    {
        foreach (MapManager map in mapPrefabList.Maps)
        {
            if (map == null) continue;

            map.tpUpID = "";
            map.tpDownID = "";
            map.tpLeftID = "";
            map.tpRightID = "";

            map.isTpUpSeted = false;
            map.isTpDownSeted = false;
            map.isTpLeftSeted = false;
            map.isTpRightSeted = false;
        }
    }
    private void RefreshInspectorForTPID()
    {
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var room = _mapGrid[x, y];
                if (room == null) continue;

                room.RefreshMapId();
            }
        }
    }
    private void DisableUnusedTeleporters()
    {
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var room = _mapGrid[x, y];
                if (room == null) continue;

                if (!room.isTpUpSeted && room.tpUp != null)
                    room.tpUp.SetActive(false);

                if (!room.isTpDownSeted && room.tpDown != null)
                    room.tpDown.SetActive(false);

                if (!room.isTpLeftSeted && room.tpLeft != null)
                    room.tpLeft.SetActive(false);

                if (!room.isTpRightSeted && room.tpRight != null)
                    room.tpRight.SetActive(false);
            }
        }
    }

    #endregion

    private void Start()
    {
        ResetAllMapPrefabs();
        ReqestMapSpawn();
        AssignTeleportIDs();
        RefreshInspectorForTPID();
        DisableUnusedTeleporters();
    }

}

public static class ListExtension
{
    public static T PickRandom<T>(this List<T> list)
    {
        return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
    }
}


