using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor.Rendering;

public class MapRpc : NetworkBehaviour
{
    [SerializeField] private MapRandomSpawner mapRandomSpawner;
    public const int MAP_SIZE = 10;

    public void UploadMapToClients(MapData[,] mapGrid)
    {
        List<NetWorkMapData> tempList = new List<NetWorkMapData>();

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                MapData map = mapGrid[x, y];
                if (map != null)
                {
                    tempList.Add(ConvertToNetworkData(map));
                }
                else
                {
                    tempList.Add(new NetWorkMapData { roomType = Enums.RoomType.NONE });
                }
            }
        }

        ReceiveMapClientRpc(tempList.ToArray()); // ✅ 배열로 변환해서 전송
    }

    [ClientRpc]
    public void ReceiveMapClientRpc(NetWorkMapData[] mapArray)
    {
        Debug.Log("[Client] 맵 정보 수신 완료");

        MapData[,] localGrid = mapRandomSpawner.GetMapGrid();

        
        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                MapData newMapData = new MapData();
                newMapData.SetGridPosition(new Vector2Int(x, y));
                newMapData.tpUpID = mapArray[x + (y * 10)].tpUpID.ToString();
                newMapData.tpDownID = mapArray[x + (y * 10)].tpDownID.ToString();
                newMapData.tpLeftID = mapArray[x + (y * 10)].tpLeftID.ToString();
                newMapData.tpRightID = mapArray[x + (y * 10)].tpRightID.ToString();

                newMapData.canConnectUp = mapArray[x + (y * 10)].canConnectUp;
                newMapData.canConnectDown = mapArray[x + (y * 10)].canConnectDown;
                newMapData.canConnectLeft = mapArray[x + (y * 10)].canConnectLeft;
                newMapData.canConnectRight = mapArray[x + (y * 10)].canConnectRight;

                newMapData.roomType = mapArray[x + (y * 10)].roomType;

                localGrid[x, y] = newMapData; 
            }
        }

        
        if (!mapRandomSpawner.isShopScene)
        {
            Debug.Log("클라이언트측 일반맵 스폰 실행");
            Debug.Log("클라이언트의 맵 텔레포트 설정");
            mapRandomSpawner.AssignTeleportIDs(localGrid);
            mapRandomSpawner.DisableUnusedTeleporters(localGrid);
        }
        else
        {
            mapRandomSpawner.RequestShopMapSpawn(localGrid);
        }

        mapRandomSpawner.NotifyPlayerWallUpdate();
        mapRandomSpawner.RequestSetPlayerPos();
    }

    //private void SetTpIDIfExists(GameObject tpObj, string id)
    //{
    //    if (tpObj == null || string.IsNullOrEmpty(id)) return;

    //    var tpManager = tpObj.GetComponent<TeleportTileManager>();
    //    if (tpManager != null)
    //        tpManager.teleportID = id;

    //    var tilemap = tpObj.GetComponent<Tilemap>();
    //    if (tilemap != null)
    //    {
    //        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
    //        {
    //            if (!tilemap.HasTile(pos)) continue;
    //            var tile = tilemap.GetTile(pos) as TileMapTelepoter;
    //            if (tile != null)
    //            {
    //                tile.teleportID = id;
    //                break;
    //            }
    //        }
    //    }
    //}



    private NetWorkMapData ConvertToNetworkData(MapData map)
    {
        return new NetWorkMapData
        {
            roomType = map.roomType,
            canConnectUp = map.canConnectUp,
            canConnectDown = map.canConnectDown,
            canConnectLeft = map.canConnectLeft,
            canConnectRight = map.canConnectRight,
            tpUpID = map.tpUpID,
            tpDownID = map.tpDownID,
            tpLeftID = map.tpLeftID,
            tpRightID = map.tpRightID
        };
    }
}