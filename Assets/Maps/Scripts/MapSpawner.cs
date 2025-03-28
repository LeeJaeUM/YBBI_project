using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapSpawner : MonoBehaviour
{
    public GameObject baseMapPrefab; // BaseMap 프리팹
    public int mapCount = 4; // 시작 맵 + 이어지는 3개
    public float mapOffset = 45;
    private Vector3 offset; // 맵 간 간격
    
    private void Start()
    {
        ReqestMapSpawn();
    }

    private void ReqestMapSpawn()
    {
        int i = 0;
        GameObject mapInstance = Instantiate(baseMapPrefab, Vector3.zero, Quaternion.identity); ;
        TeleportManager tpEntress = mapInstance.transform.Find("TpEntress")?.GetComponent<TeleportManager>(); ;
        Vector3 spawnPos = Vector3.zero;

        for (i = 0; i < mapCount; i++)
        {
            switch (i)
            {
                case 0:
                    offset = new Vector3(0, 0, 0);
                    break;

                case 1:
                    offset = new Vector3(0, mapOffset, 0);
                    break;

                case 2:
                    offset = new Vector3(mapOffset, 0, 0);
                    break;

                case 3:
                    offset = new Vector3(mapOffset, mapOffset, 0);
                    break;


                default:
                    break;
            }
            spawnPos = offset;

            mapInstance = Instantiate(baseMapPrefab, spawnPos, Quaternion.identity);

            tpEntress = mapInstance.transform.Find("TpEntressLeft")?.GetComponent<TeleportManager>();
            if (tpEntress != null)
            {
                tpEntress.teleportID = ((char)('A' + i)).ToString(); // A, B, C, D...
                Debug.Log($"{tpEntress.transform.position}텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
            }

            // 텔레포트 ID 자동 설정
            tpEntress = mapInstance.transform.Find("TpEntressRight")?.GetComponent<TeleportManager>();
            if (tpEntress != null)
            {
                tpEntress.teleportID = ((char)('A' + i + 1)).ToString(); // A, B, C, D...
                Debug.Log($"{tpEntress.transform.position}텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
            }
        }

        if (tpEntress.teleportID == ((char)('A' + i)).ToString())
        {
            tpEntress.teleportID = ((char)('A')).ToString(); // A, B, C, D...
            Debug.Log($"마지막 텔레포트 ID 변경됨,  ID : {tpEntress.teleportID}");
        }
    }
}
