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
        ReceiveMapClientRpc(); // ✅ 배열로 변환해서 전송
    }

    [ClientRpc]
    public void ReceiveMapClientRpc()
    {
        mapRandomSpawner.NotifyPlayerWallUpdate();
        mapRandomSpawner.RequestSetPlayerPos();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnAllMapsServerRpc()
    {
        mapRandomSpawner.DespawnAllMaps();
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