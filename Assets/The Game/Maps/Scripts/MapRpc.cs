using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

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

        int index = 0;
        MapData[,] localGrid = mapRandomSpawner.GetMapGrid();

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                if (index >= mapArray.Length) return;

                NetWorkMapData data = mapArray[index++];
                if (data.roomType == Enums.RoomType.NONE) continue;

                MapData map = localGrid[x, y];
                if (map == null) continue;

                // ID 복원
                map.tpUpID = data.tpUpID.ToString();
                map.tpDownID = data.tpDownID.ToString();
                map.tpLeftID = data.tpLeftID.ToString();
                map.tpRightID = data.tpRightID.ToString();

                // ID 기반으로 텔레포트 설정
                SetTpIDIfExists(map.tpUp, map.tpUpID);
                SetTpIDIfExists(map.tpDown, map.tpDownID);
                SetTpIDIfExists(map.tpLeft, map.tpLeftID);
                SetTpIDIfExists(map.tpRight, map.tpRightID);

                // ID 없으면 비활성화
                if (string.IsNullOrEmpty(map.tpUpID) && map.tpUp != null)
                    map.tpUp.SetActive(false);
                if (string.IsNullOrEmpty(map.tpDownID) && map.tpDown != null)
                    map.tpDown.SetActive(false);
                if (string.IsNullOrEmpty(map.tpLeftID) && map.tpLeft != null)
                    map.tpLeft.SetActive(false);
                if (string.IsNullOrEmpty(map.tpRightID) && map.tpRight != null)
                    map.tpRight.SetActive(false);
            }
        }

        Debug.Log("[Client] 텔레포트 ID 복원 완료");
    }

    private void SetTpIDIfExists(GameObject tpObj, string id)
    {
        if (tpObj == null || string.IsNullOrEmpty(id)) return;

        var tpManager = tpObj.GetComponent<TeleportTileManager>();
        if (tpManager != null)
            tpManager.teleportID = id;

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
                    break;
                }
            }
        }
    }



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