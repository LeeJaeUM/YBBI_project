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

    [HideInInspector] public bool isAllEnemySpawned = false;
    [HideInInspector] public bool isRoomCleared = true;

    private MapManager mapData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapData = mapOBJ.GetComponent<MapManager>();
    }

    public void RequestEnterFight()
    {
        if (mapData.roomType == RoomType.Enemy && enemySpawnTileMap != null)
        {
            List<EnemySpawnTile> spawnTileList = new List<EnemySpawnTile>();
            BoundsInt bounds = enemySpawnTileMap.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase baseTile = enemySpawnTileMap.GetTile(pos);
                if (baseTile is EnemySpawnTile currenTEnemySpawnTile)
                {
                    if(!currenTEnemySpawnTile.isEnemySpawned)
                    {
                        Vector3 spawnPos = enemySpawnTileMap.CellToWorld((Vector3Int.FloorToInt(pos)));
                        GameObject newEnemy = Instantiate(gameObject, spawnPos, Quaternion.identity, this.transform);
                        currenTEnemySpawnTile.isEnemySpawned = true;
                    }
                }
            }

            isRoomCleared = false;
        }
    }

        // Update is called once per frame
    void Update()
    {
        
    }
}
