using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawnManager : MonoBehaviour
{

    [Header("부모 오브젝트")]
    [SerializeField] private GameObject mapOBJ;

    [Header("적이 스폰될 지점이 찍힌 타일맵과 적 리스트")]
    [SerializeField] private Tilemap enemySpawnTileMap;
    [SerializeField] private EnemyListSO enemyList;


    [HideInInspector] public List<GameObject> spawnedEnemyList = new List<GameObject>();
    private bool isRoomCleared = true;
    private bool isSpawnActivedOnce = false;
    private MapManager mapData;

    private Dictionary<Vector3Int, bool> EnemySpawnTileStatus = new();

    public void RequestEnterFight()
    {
        if(!isSpawnActivedOnce)
        {
            if (mapData.roomType == Enums.RoomType.Enemy && enemySpawnTileMap != null)
            {
                Debug.Log("전투속행");
                EnteredNomalEnemyRoom();
                isRoomCleared = false;
                isSpawnActivedOnce = true;
                RequestTurnOffTeleportals();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RequestEnterFight();
            Debug.Log("전투방 감지됨");
        }
    }

    private void EnteredNomalEnemyRoom()
    {
        BoundsInt bounds = enemySpawnTileMap.cellBounds;

        foreach (Vector3Int tilePos in bounds.allPositionsWithin)
        {
            TileBase baseTile = enemySpawnTileMap.GetTile(tilePos);
            if (baseTile is EnemySpawnTile)
            {
                // 좌표 기반 상태 체크
                if (EnemySpawnTileStatus.ContainsKey(tilePos) && EnemySpawnTileStatus[tilePos])
                    continue;

                Vector3 spawnPos = enemySpawnTileMap.CellToWorld(tilePos);

                GameObject enemyInstance = Instantiate(EnemyPrefabSelect(Enums.EnemeyValue.NomalEnemy1), spawnPos, Quaternion.identity, transform);
                spawnedEnemyList.Add(enemyInstance);
                EnemySpawnTileStatus[tilePos] = true; // 스폰됨 표시

                EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector3Int capturedTilePos = tilePos;
                    GameObject capturedEnemy = enemyInstance;

                    enemyHealth.OnDie += () =>
                    {
                        EnemySpawnTileStatus[capturedTilePos] = false;
                        spawnedEnemyList.Remove(capturedEnemy);
                        CheckAllEnemyDieInRoom();
                    };
                }
            }
        }
    }

    private GameObject EnemyPrefabSelect(Enums.EnemeyValue var)
    {
        //실험적 기능으로 EnemyHealth를 가진 프리팹만 가져오게 만들어 랜덤 스폰하게 하려고 시도한 흔적임
        //List<GameObject> filteredEnemies = new List<GameObject>();
        //
        //foreach (GameObject enemyPrefab in enemyList.Enemy)
        //{
        //    if (enemyPrefab != null && enemyPrefab.GetComponent<EnemyHealth>() != null)
        //    {
        //        filteredEnemies.Add(enemyPrefab);
        //    }
        //}

        return enemyList.Enemy[(int)var];
    }

    public void CheckAllEnemyDieInRoom()
    {
        Debug.Log("맵 클리어 확인시도");
        if (spawnedEnemyList == null || spawnedEnemyList.Count == 0)
        {
            Debug.Log("맵 클리어 확인됨");
            isRoomCleared = true;
            RequestTurnOnTeleportals();
        }    
    }

    private void RequestTurnOffTeleportals()
    {
        MapRandomSpawner spawner = mapOBJ.GetComponentInParent<MapRandomSpawner>();
        if(spawner == null) 
        {
            Debug.Log("텔레포트를 끌 오브젝트를 찾지 못함.");
            return;
        }
        spawner.TurnOffTPs(mapOBJ);
    }
    private void RequestTurnOnTeleportals()
    {
        MapRandomSpawner spawner = mapOBJ.GetComponentInParent<MapRandomSpawner>();
        if (spawner == null)
        {
            Debug.Log("텔레포트를 끌 오브젝트를 찾지 못함.");
            return;
        }
        spawner.TurnOnTPs(mapOBJ);
    }

    private void Awake()
    {
        mapData = mapOBJ.GetComponent<MapManager>();
    }
}
