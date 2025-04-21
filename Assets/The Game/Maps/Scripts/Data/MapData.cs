using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum RoomType
{
    Normal,
    Start,
    Boss,
    Shop,
    Treasure
    // 필요시 확장
}

public class MapData : MonoBehaviour
{
    public GameObject roomPrefab;
    public RoomType roomType;

    // 각 방향에 텔레포트 가능한지
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

    private void Start()
    {
        tpUp = transform.Find("TpEntressUp")?.gameObject;
        tpDown = transform.Find("TpEntressDown")?.gameObject;
        tpLeft = transform.Find("TpEntressLeft")?.gameObject;
        tpRight = transform.Find("TpEntressRight")?.gameObject;
        Debug.Log($"[RoomInfo] {gameObject.name} 포탈 오브젝트 자동 할당 완료");
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