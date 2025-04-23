using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/EnemySpawnTile")]
public class EnemySpawnTile : Tile
{
    public bool isEnemySpawned = false;
    public bool isEnemyDead = false; // "A", "B" 같은 이름으로 쌍을 구분
}