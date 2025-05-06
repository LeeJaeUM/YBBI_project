using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapData : MonoBehaviour
{
    public GameObject roomPrefab;
    public Enums.RoomType roomType;

    [Header("맵이 연결 가능하지에 대한 여부")]
    public bool canConnectUp;
    public bool canConnectDown;
    public bool canConnectLeft;
    public bool canConnectRight;

    [Header("텔레포트 타일")]
    public GameObject tpUp;
    public string tpUpID;
    public bool isTpUpSeted = false;
    public GameObject tpDown;
    public string tpDownID;
    public bool isTpDownSeted = false;
    public GameObject tpLeft;
    public string tpLeftID;
    public bool isTpLeftSeted = false;
    public GameObject tpRight;
    public string tpRightID;
    public bool isTpRightSeted = false;

    public Vector2Int gridPos;

    public void SetGridPosition(Vector2Int pos)
    {
        gridPos = pos;
    }

    public void RefreshMapId()
    {
        if(tpUp != null)
        {
            TeleportTileManager telepoterTile = tpUp.GetComponent<TeleportTileManager>();
            tpUpID = telepoterTile.teleportID;
        }
        else
        {
            Debug.Log("tpUpID가 null임");
        }
        if (tpDown != null)
        {
            TeleportTileManager telepoterTile = tpDown.GetComponent<TeleportTileManager>();
            tpDownID = telepoterTile.teleportID;
        }
        else
        {
            Debug.Log("tpDownID가 null임");
        }
        if (tpLeft != null)
        {
            TeleportTileManager telepoterTile = tpLeft.GetComponent<TeleportTileManager>();
            tpLeftID = telepoterTile.teleportID;
        }
        else
        {
            Debug.Log("tpLeftID가 null임");
        }
        if (tpRight != null)
        {
            TeleportTileManager telepoterTile = tpRight.GetComponent<TeleportTileManager>();
            tpRightID = telepoterTile.teleportID;
        }
        else
        {
            Debug.Log("tpRightID가 null임");
        }
    }
}