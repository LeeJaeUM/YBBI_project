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
    private MapData[,] _mapGrid = new MapData[MAP_SIZE, MAP_SIZE];

    Vector2Int[] _directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };


    #region 맵생성로직

    private void ReqestMapSpawn()
    {
        Vector2Int center = GetMapListGridCenter();
        MapData startRoom = GetRandomRoom(RoomType.Start);
        _mapGrid[center.x, center.y] = startRoom;
        Instantiate(startRoom.gameObject, GridToWorld(center), Quaternion.identity, mapSpawnGrid.transform);

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(center);
        int placedRoomCount = 1;

        while (frontier.Count > 0 && placedRoomCount < mapCount - 1)
        {
            Vector2Int currentPos = frontier.Dequeue();
            MapData currentRoom = _mapGrid[currentPos.x, currentPos.y];

            List<Vector2Int> dirList = new List<Vector2Int>(_directions);
            Shuffle(dirList); // 방향 무작위화

            foreach (var dir in dirList)
            {
                Vector2Int nextPos = currentPos + dir;
                if (!IsValidPos(nextPos)) continue;
                if (_mapGrid[nextPos.x, nextPos.y] != null) continue;
                if (FormsSquare(nextPos)) continue;

                MapData newRoom = GetRandomCompatibleRoom(RoomType.Normal, dir, currentRoom);
                if (newRoom == null) continue;

                _mapGrid[nextPos.x, nextPos.y] = newRoom;
                Instantiate(newRoom.gameObject, GridToWorld(nextPos), Quaternion.identity, mapSpawnGrid.transform);
                frontier.Enqueue(nextPos);
                placedRoomCount++;

                if (placedRoomCount >= mapCount - 1)
                    break; // 목표 개수 도달하면 바로 종료
            }
        }

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                if (_mapGrid[x, y] != null)
                {
                    foreach (var dir in _directions)
                    {
                        Vector2Int bossPos = new Vector2Int(x, y) + dir;
                        if (!IsValidPos(bossPos)) continue;
                        if (_mapGrid[bossPos.x, bossPos.y] != null) continue;
                        if (FormsSquare(bossPos)) continue;

                        MapData bossRoom = GetRandomCompatibleRoom(RoomType.Boss, dir, _mapGrid[x, y]);
                        if (bossRoom != null)
                        {
                            _mapGrid[bossPos.x, bossPos.y] = bossRoom;
                            Instantiate(bossRoom.gameObject, GridToWorld(bossPos), Quaternion.identity, mapSpawnGrid.transform);
                            return;
                        }
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

    private MapData GetRandomRoom(RoomType type)
    {
        List<MapData> list = mapPrefabList.Maps.FindAll(m => m.roomType == type);
        return list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
    }

    private MapData GetRandomCompatibleRoom(RoomType type, Vector2Int fromDir, MapData fromRoom)
    {
        List<MapData> list = mapPrefabList.Maps.FindAll(m => m.roomType == type);

        if (fromRoom == null)
            return list.PickRandom(); // fromRoom이 null이면 아무거나 리턴

        return list.FindAll(m => IsCompatible(fromDir, fromRoom, m)).PickRandom();
    }

    private bool IsCompatible(Vector2Int dir, MapData from, MapData to)
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

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var room = _mapGrid[x, y];
                if (room == null) continue;
                Vector2Int currentPos = new Vector2Int(x, y);

                // 위 방향 연결
                if (room.canConnectUp)
                {
                    Vector2Int neighborPos = currentPos + Vector2Int.up;
                    if (IsValidPos(neighborPos) && _mapGrid[neighborPos.x, neighborPos.y] != null && !_mapGrid[neighborPos.x, neighborPos.y].isTpUpSeted)
                    {
                        string tpID = $"TP_{stageNum}_{idCounter++}";
                        SetTeleportID(room.tpUp, tpID);
                        SetTeleportID(_mapGrid[neighborPos.x, neighborPos.y].tpDown, tpID);
                        room.isTpUpSeted = true;
                        _mapGrid[neighborPos.x, neighborPos.y].isTpDownSeted = true;
                    }
                }

                // 아래 방향 연결
                if (room.canConnectDown)
                {
                    Vector2Int neighborPos = currentPos + Vector2Int.down;
                    if (IsValidPos(neighborPos) && _mapGrid[neighborPos.x, neighborPos.y] != null && !_mapGrid[neighborPos.x, neighborPos.y].isTpDownSeted)
                    {
                        string tpID = $"TP_{stageNum}_{idCounter++}";
                        SetTeleportID(room.tpDown, tpID);
                        SetTeleportID(_mapGrid[neighborPos.x, neighborPos.y].tpUp, tpID);
                        room.isTpDownSeted = true;
                        _mapGrid[neighborPos.x, neighborPos.y].isTpUpSeted = true;
                    }
                }

                // 왼쪽 방향 연결
                if (room.canConnectLeft)
                {
                    Vector2Int neighborPos = currentPos + Vector2Int.left;
                    if (IsValidPos(neighborPos) && _mapGrid[neighborPos.x, neighborPos.y] != null && !_mapGrid[neighborPos.x, neighborPos.y].isTpLeftSeted)
                    {
                        string tpID = $"TP_{stageNum}_{idCounter++}";
                        SetTeleportID(room.tpLeft, tpID);
                        SetTeleportID(_mapGrid[neighborPos.x, neighborPos.y].tpRight, tpID);
                        room.isTpLeftSeted = true;
                        _mapGrid[neighborPos.x, neighborPos.y].isTpRightSeted = true;
                    }
                }

                // 오른쪽 방향 연결
                if (room.canConnectRight)
                {
                    Vector2Int neighborPos = currentPos + Vector2Int.right;
                    if (IsValidPos(neighborPos) && _mapGrid[neighborPos.x, neighborPos.y] != null && !_mapGrid[neighborPos.x, neighborPos.y].isTpRightSeted)
                    {
                        string tpID = $"TP_{stageNum}_{idCounter++}";
                        SetTeleportID(room.tpRight, tpID);
                        SetTeleportID(_mapGrid[neighborPos.x, neighborPos.y].tpLeft, tpID);
                        room.isTpRightSeted = true;
                        _mapGrid[neighborPos.x, neighborPos.y].isTpLeftSeted = true;
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
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos) as TelepoterTile;
                if (tile != null)
                    tile.teleportID = id;
            }
        }
    }
    #endregion


    #region 맵초기화 로직
    private void ResetAllMapPrefabs()
    {
        foreach (MapData map in mapPrefabList.Maps)
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
    #endregion

    private void Start()
    {
        ResetAllMapPrefabs();
        ReqestMapSpawn();
        AssignTeleportIDs();
        RefreshInspectorForTPID();
    }

}

public static class ListExtension
{
    public static T PickRandom<T>(this List<T> list)
    {
        return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
    }
}


