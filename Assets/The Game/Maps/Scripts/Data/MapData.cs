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
    public GameObject tpDown;
    public string tpDownID;
    public GameObject tpLeft;
    public string tpLeftID;
    public GameObject tpRight;
    public string tpRightID;

    private void Start()
    {
        tpUp = transform.Find("TpEntressUp")?.gameObject;
        tpDown = transform.Find("TpEntressDown")?.gameObject;
        tpLeft = transform.Find("TpEntressLeft")?.gameObject;
        tpRight = transform.Find("TpEntressRight")?.gameObject;
        Debug.Log($"[RoomInfo] {gameObject.name} 포탈 오브젝트 자동 할당 완료");

    }


    public void ApplyTeleportObjectVisibility(Dictionary<Vector2Int, bool> connectionMap)
    {
        if (tpUp != null) tpUp.SetActive(connectionMap.TryGetValue(Vector2Int.up, out bool u) && u);
        if (tpDown != null) tpDown.SetActive(connectionMap.TryGetValue(Vector2Int.down, out bool d) && d);
        if (tpLeft != null) tpLeft.SetActive(connectionMap.TryGetValue(Vector2Int.left, out bool l) && l);
        if (tpRight != null) tpRight.SetActive(connectionMap.TryGetValue(Vector2Int.right, out bool r) && r);
    }

#if UNITY_EDITOR
    [ContextMenu("텔레포트 방향 자동 감지")]
    public void AutoDetectTeleportDirections()
    {
        canConnectUp = transform.Find("TpEntressUp") != null;
        canConnectDown = transform.Find("TpEntressDown") != null;
        canConnectLeft = transform.Find("TpEntressLeft") != null;
        canConnectRight = transform.Find("TpEntressRight") != null;

        Debug.Log($"[RoomInfo] 텔레포트 방향 자동 감지 완료: {gameObject.name}");
    }
#endif
}