using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapRandomSpawner : NetworkBehaviour
{
    [SerializeField] private MapRpc mapRpc;
    [SerializeField] private Grid mapSpawnGrid;
    [SerializeField] private MapListSO mapPrefabList; // BaseMap 프리팹
    [SerializeField] private int mapCount = 13;
    [SerializeField] private int stageNum = 1;
    

    public bool isShopScene = false;
    public const int MAP_SIZE = 10; //MAP_SIZE * MAP_SIZE의 크기의 맵 배열
    private PlayerJobPrefabManager jobMgr;
    private MapData[,] _mapGrid = new MapData[MAP_SIZE, MAP_SIZE];

    Vector2Int[] _directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };


    #region 맵생성로직

    public void ReqestMapSpawn()
    {
        Vector2Int center = GetMapListGridCenter();
        MapData startRoom = GetRandomRoom(Enums.RoomType.Start);

        GameObject startObj = Instantiate(startRoom.gameObject, GridToWorld(center), Quaternion.identity);
        Debug.Log($"[HOST] {startObj.name} 생성됨, 위치: {startObj.transform.position}");
        MapData startData = startObj.GetComponent<MapData>();
        startData.SetGridPosition(center);
        startObj.GetComponent<NetworkObject>().Spawn();
        _mapGrid[center.x, center.y] = startData;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(center);
        int placedRoomCount = 1;

        while (frontier.Count > 0 && placedRoomCount < mapCount - 1)
        {
            Vector2Int currentPos = frontier.Dequeue();
            MapData currentRoom = _mapGrid[currentPos.x, currentPos.y];

            List<Vector2Int> dirList = new List<Vector2Int>(_directions);
            Shuffle(dirList);

            foreach (var dir in dirList)
            {
                Vector2Int nextPos = currentPos + dir;
                if (!IsValidPos(nextPos)) continue;
                if (_mapGrid[nextPos.x, nextPos.y] != null) continue;
                if (FormsSquare(nextPos)) continue;



                MapData newRoom = GetRandomCompatibleRoom(Enums.RoomType.Normal, dir, currentRoom);
                if (newRoom == null) continue;

                GameObject newObj = Instantiate(newRoom.gameObject, GridToWorld(nextPos), Quaternion.identity);
                Debug.Log($"[HOST] {newObj.name} 생성됨, 위치: {newObj.transform.position}");
                MapData newData = newObj.GetComponent<MapData>();
                newData.SetGridPosition(nextPos);
                newObj.GetComponent<NetworkObject>().Spawn();
                if (newData.roomType == Enums.RoomType.Enemy)
                {
                    EnemySpawnManager enemySpawnManager = newObj.GetComponentInChildren<EnemySpawnManager>();
                    enemySpawnManager.SetUpEnemySpawnManager();
                }

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

                    MapData bossRoom = GetRandomCompatibleRoom(Enums.RoomType.Boss, dir, _mapGrid[x, y]);
                    if (bossRoom != null)
                    {
                        GameObject bossObj = Instantiate(bossRoom.gameObject, GridToWorld(bossPos), Quaternion.identity);
                        Debug.Log($"[HOST] {bossObj.name} 생성됨, 위치: {bossObj.transform.position}");
                        MapData bossData = bossObj.GetComponent<MapData>();
                        bossData.SetGridPosition(bossPos);

                        EnemySpawnManager enemySpawnManager = bossObj.GetComponentInChildren<EnemySpawnManager>();
                        enemySpawnManager.SetUpEnemySpawnManager();

                        bossObj.GetComponent<NetworkObject>().Spawn();
                        _mapGrid[bossPos.x, bossPos.y] = bossData;
                        return;
                    }
                }
            }
        }
    }

    public void RequestShopMapSpawn(MapData[,] mapGrid)
    {
        Vector2Int center = GetMapListGridCenter();
        MapData ShopRoom = GetRandomRoom(Enums.RoomType.Shop);

        GameObject shopRoom = Instantiate(ShopRoom.gameObject, GridToWorld(center), Quaternion.identity, mapSpawnGrid.transform);
        MapData shopRoomData = shopRoom.GetComponent<MapData>();
        mapGrid[center.x, center.y] = shopRoomData;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(center);
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

    private MapData GetRandomRoom(Enums.RoomType type)
    {
        List<MapData> list = mapPrefabList.Maps.FindAll(m => m.roomType == type);
        return list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
    }

    private MapData GetRandomCompatibleRoom(Enums.RoomType type, Vector2Int fromDir, MapData fromRoom)
    {
        List<MapData> list;
        if (type == Enums.RoomType.Normal)
        {
            list = mapPrefabList.Maps.FindAll(m => m.roomType == Enums.RoomType.Normal || m.roomType == Enums.RoomType.Enemy);
        }
        else
        {
            list = mapPrefabList.Maps.FindAll(m => m.roomType == Enums.RoomType.Boss);
        }

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
    public void AssignTeleportIDs(MapData[,] mapGrid)
    {
        int idCounter = 0;
        MapData BossRoom;
        Vector2Int[] teleportDirs = { Vector2Int.down, Vector2Int.left }; // 한쪽만 처리해서 중복 방지

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var currentRoom = mapGrid[x, y];
                if (currentRoom == null) continue;
                Vector2Int currentPos = new Vector2Int(x, y);

                foreach (var dir in teleportDirs)
                {
                    Vector2Int neighborPos = currentPos + dir;
                    if (!IsValidPos(neighborPos)) continue;

                    var neighborRoom = mapGrid[neighborPos.x, neighborPos.y];
                    if (neighborRoom == null) continue;

                    if (currentRoom.roomType == Enums.RoomType.NONE || neighborRoom.roomType == Enums.RoomType.NONE) continue;

                    string tpID = $"TP_{stageNum}_{idCounter}";

                    if (currentRoom.roomType != Enums.RoomType.Boss || neighborRoom.roomType != Enums.RoomType.Boss)
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
                    else if (currentRoom.roomType == Enums.RoomType.Boss)
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
                    else if (neighborRoom.roomType == Enums.RoomType.Boss)
                    {
                        if (dir == Vector2Int.down &&
                        !currentRoom.isTpDownSeted && !currentRoom.isTpDownSeted &&
                        neighborRoom.canConnectDown && !neighborRoom.isTpDownSeted)
                        {
                            SetTeleportID(currentRoom.tpDown, tpID);
                            SetTeleportID(neighborRoom.tpDown, tpID);
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
                            SetTeleportID(neighborRoom.tpDown, tpID);
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
    public void ResetAllMapPrefabs()
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
    public void RefreshInspectorForTPID(MapData[,] mapGrid)
    {
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var room = mapGrid[x, y];
                if (room == null) continue;

                room.RefreshMapId();
            }
        }
    }
    public void DisableUnusedTeleporters(MapData[,] mapGrid)
    {
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                var room = mapGrid[x, y];
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

    #region 기타 로직
    private Vector2Int? FindMapGridPosition(GameObject mapObject)
    {
        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                if (_mapGrid[x, y] != null && _mapGrid[x, y].gameObject == mapObject)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null; // 못 찾았을 경우
    }

    public void TurnOffTPs(GameObject mapObject)
    {
        Vector2Int? mapOBJGridPos = FindMapGridPosition(mapObject);
        if (mapOBJGridPos == null) return;

        MapData room = _mapGrid[mapOBJGridPos.Value.x, mapOBJGridPos.Value.y];
        if (room == null) return;

        if (room.tpUp != null)
            room.tpUp.SetActive(false);

        if (room.tpDown != null)
            room.tpDown.SetActive(false);

        if (room.tpLeft != null)
            room.tpLeft.SetActive(false);

        if (room.tpRight != null)
            room.tpRight.SetActive(false);
    }

    public void TurnOnTPs(GameObject mapObject)
    {
        Vector2Int? mapOBJGridPos = FindMapGridPosition(mapObject);
        if (mapOBJGridPos == null) return;

        MapData room = _mapGrid[mapOBJGridPos.Value.x, mapOBJGridPos.Value.y];
        if (room == null) return;

        if (room.isTpUpSeted && room.tpUp != null)
            room.tpUp.SetActive(true);

        if (room.isTpDownSeted && room.tpDown != null)
            room.tpDown.SetActive(true);

        if (room.isTpLeftSeted && room.tpLeft != null)
            room.tpLeft.SetActive(true);

        if (room.isTpRightSeted && room.tpRight != null)
            room.tpRight.SetActive(true);
    }
    #endregion

    public void NotifyPlayerWallUpdate()
    {
        TheGamePlayerMover[] movers = FindObjectsOfType<TheGamePlayerMover>();
        foreach (var mover in movers)
        {
            mover.ForceUpdateWallTilemaps();
        }
    }

    public void RequestSetPlayerPos()
    {
        PlayerPosRPC[] posRpcs = FindObjectsOfType<PlayerPosRPC>();
        foreach (var posRpc in posRpcs)
        {
            Vector3 _pos = GridToWorld(GetMapListGridCenter());
            posRpc.TeleportRequest(_pos);
        }
    }


    public void RebuildMapArray()
    {
        MapData[] allMaps = FindObjectsOfType<MapData>();

        foreach (var map in allMaps)
        {
            Vector2Int pos = map.gridPos;
            if (pos.x < 0 || pos.x >= MAP_SIZE || pos.y < 0 || pos.y >= MAP_SIZE)
                continue;

            _mapGrid[pos.x, pos.y] = map;
        }

        Debug.Log("[클라이언트] 맵 배열 재구성 완료");
    }

    public MapData[,] GetMapGrid()
    {
        return _mapGrid;
    }

    public void SetMapGrid(MapData[,] newMapData)
    {
        _mapGrid = newMapData;
    }

    #region 시작

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Debug.Log("서버 맵 스폰 실행");
        ResetAllMapPrefabs();

        if (!isShopScene)
        {
            Debug.Log("서버측 일반맵 스폰 실행");
            ReqestMapSpawn();
            AssignTeleportIDs(_mapGrid);
            RefreshInspectorForTPID(_mapGrid);
            DisableUnusedTeleporters(_mapGrid);

/*            EnemySpawnManager[] enemySpawnManagers = GetComponentsInChildren<EnemySpawnManager>();
            foreach (var enemySpawnManager in enemySpawnManagers)
            {
                enemySpawnManager.SetUpEnemySpawnManager();
            }*/
        }
        else
        {
            RequestShopMapSpawn(_mapGrid);
        }

        NotifyPlayerWallUpdate();
        RequestSetPlayerPos();

        mapRpc.UploadMapToClients(GetMapGrid());
    }

}
#endregion

public static class ListExtension
{
    public static T PickRandom<T>(this List<T> list)
    {
        return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
    }
}


