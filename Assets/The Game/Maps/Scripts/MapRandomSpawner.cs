using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapRandomSpawner : MonoBehaviour
{
    public MapListSO mapPrefabList; // BaseMap 프리팹
    public int mapCount = 4; // 시작 맵 + 이어지는 3개
    public float mapOffset = 45;
    private Vector3 offset; // 맵 간 간격

    private void Start()
    {
        ReqestMapSpawn();
    }

    private void ReqestMapSpawn()
    {
        if (mapPrefabList.Maps.Count < 2)
        {
            Debug.LogError("맵 리스트에 최소 2개 이상의 프리팹이 필요합니다.");
            return;
        }

        for (int i = 0; i < mapCount; i++)
        {
            offset = new Vector3(i * mapOffset, 0, 0);
            GameObject selectedPrefab;

            if (i == 0)
            {
                // 시작 맵
                var startRoom = mapPrefabList.Maps.FindAll(p => p.GetComponent<MapData>().roomType == RoomType.Start);
                // 마지막 맵
                selectedPrefab = startRoom[0].gameObject;
            }
            else if (i == mapCount - 1)
            {
                var bossRoom = mapPrefabList.Maps.FindAll(p => p.GetComponent<MapData>().roomType == RoomType.Boss);
                // 마지막 맵
                selectedPrefab = bossRoom[0].gameObject;
            }
            else
            {
                // 중간 맵 (랜덤 선택)
                int randomIndex = Random.Range(2, mapPrefabList.Maps.Count);
                selectedPrefab = mapPrefabList.Maps[randomIndex].gameObject;
            }

            GameObject mapInstance = Instantiate(selectedPrefab, offset, Quaternion.identity);

            TeleportTileManager tpEntress = mapInstance.transform.Find("TpEntressLeft")?.GetComponent<TeleportTileManager>();
            if (tpEntress != null)
            {
                tpEntress.teleportID = ((char)('A' + i)).ToString(); // A, B, C, D...
                Debug.Log($"{tpEntress.transform.position}텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
            }

            // 텔레포트 ID 자동 설정
            tpEntress = mapInstance.transform.Find("TpEntressRight")?.GetComponent<TeleportTileManager>();
            if (tpEntress != null)
            {
                tpEntress.teleportID = ((char)('A' + i + 1)).ToString(); // A, B, C, D...
                Debug.Log($"{tpEntress.transform.position}텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
            }

            if (tpEntress.teleportID == ((char)('A' + mapCount)).ToString())
            {
                tpEntress.teleportID = ((char)('A')).ToString(); // A, B, C, D...
                Debug.Log($"마지막 텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
            }
        }
    }
}


