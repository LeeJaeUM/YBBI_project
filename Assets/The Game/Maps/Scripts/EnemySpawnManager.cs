using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private MapData mapData;

    private Dictionary<Vector3Int, bool> EnemySpawnTileStatus = new();
    private Dictionary<GameObject, Action> EnemyDeathHandlers = new();


    public void RequestEnterFight()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if(!isSpawnActivedOnce)
        {
            if (mapData.roomType == Enums.RoomType.Enemy && enemySpawnTileMap != null)
            {
                Debug.Log("전투속행");
                EnteredNomalEnemyRoom();
                isRoomCleared = false;
                isSpawnActivedOnce = true;
                //RequestTurnOffTeleportals();
            }
            else if (mapData.roomType == Enums.RoomType.Boss && enemySpawnTileMap != null)
            {
                Debug.Log("보스방 전투속행");
                EnteredBossRoom();
                isRoomCleared = false;
                isSpawnActivedOnce = true;
                //RequestTurnOffTeleportals();
            }
        }
    }
    private void EnteredBossRoom()
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

                GameObject enemyInstance = Instantiate(EnemyPrefabSelect(Enums.EnemeyValue.Boss), spawnPos, Quaternion.identity, transform);
                enemyInstance.GetComponent<NetworkObject>().Spawn();
                spawnedEnemyList.Add(enemyInstance);
                EnemySpawnTileStatus[tilePos] = true; // 스폰됨 표시

                EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector3Int capturedTilePos = tilePos;
                    GameObject capturedEnemy = enemyInstance;

                    Action deathHandler = () =>
                    {
                        EnemySpawnTileStatus[capturedTilePos] = false;
                        spawnedEnemyList.Remove(capturedEnemy);
                        EnemyDeathHandlers.Remove(capturedEnemy);
                        CheckBossDieInRoom();
                    };

                    EnemyDeathHandlers[capturedEnemy] = deathHandler;
                    enemyHealth.OnDie += deathHandler;
                }
                break;
            }
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
                enemyInstance.GetComponent<NetworkObject>().Spawn();
                spawnedEnemyList.Add(enemyInstance);
                EnemySpawnTileStatus[tilePos] = true; // 스폰됨 표시

                EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector3Int capturedTilePos = tilePos;
                    GameObject capturedEnemy = enemyInstance;

                    Action deathHandler = () =>
                    {
                        EnemySpawnTileStatus[capturedTilePos] = false;
                        spawnedEnemyList.Remove(capturedEnemy);
                        EnemyDeathHandlers.Remove(capturedEnemy);
                        CheckAllEnemyDieInRoom();
                    };

                    EnemyDeathHandlers[capturedEnemy] = deathHandler;
                    enemyHealth.OnDie += deathHandler;
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

    public void UnsubscribeAllEnemyDeaths()
    {
        foreach (var pair in EnemyDeathHandlers)
        {
            GameObject enemy = pair.Key;
            Action handler = pair.Value;

            if (enemy != null)
            {
                var health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.OnDie -= handler;
                    Debug.Log($"[EnemySpawnManager] {enemy.name} 구독 해지됨");
                }
            }
        }

        EnemyDeathHandlers.Clear(); // 딕셔너리도 초기화
    }

    public void CheckBossDieInRoom()
    {
        Debug.Log("보스방 클리어 확인시도");
        if (spawnedEnemyList == null || spawnedEnemyList.Count == 0)
        {
            Debug.Log("보스방 클리어 확인됨");
            isRoomCleared = true;
            RequestTurnOnTeleportals();
            UnsubscribeAllEnemyDeaths();
        }
    }

    public void CheckAllEnemyDieInRoom()
    {
        Debug.Log("맵 클리어 확인시도");
        if (spawnedEnemyList == null || spawnedEnemyList.Count == 0)
        {
            Debug.Log("맵 클리어 확인됨");
            isRoomCleared = true;
            RequestTurnOnTeleportals();
            UnsubscribeAllEnemyDeaths();
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
        if (mapData.roomType == Enums.RoomType.Boss)
        {
            NextSceneTileManager nextSceneTileManager = mapOBJ.GetComponentInChildren<NextSceneTileManager>();
            if (nextSceneTileManager == null)
            {
                Debug.Log("다음씬 타일의 nextSceneTileManager를 찾지 못함.");
                return;
            }
            nextSceneTileManager.gameObject.SetActive(false);
        }
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
        if (mapData.roomType == Enums.RoomType.Boss)
        {
            NextSceneTileManager nextSceneTileManager = mapOBJ.GetComponentInChildren<NextSceneTileManager>(true);
            if (nextSceneTileManager == null)
            {
                Debug.Log("다음씬 타일의 nextSceneTileManager를 찾지 못함.");
                return;
            }
            Debug.Log("[EnemySpawnManager] 다음씬 타일 다시 활성화됨: " + nextSceneTileManager.gameObject.name);
            nextSceneTileManager.gameObject.SetActive(true);
        }
    }

    public void SetUpEnemySpawnManager()
    {
        mapData = mapOBJ.GetComponent<MapData>();
        Debug.Log("[EnemySpawnManager] mapData 연결됨: " + mapData);
    }
}
