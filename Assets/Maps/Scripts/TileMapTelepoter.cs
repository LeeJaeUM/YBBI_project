using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/TileMapTelepoter")]
public class TileMapTelepoter : Tile
{
    public string teleportID; // "A", "B" 같은 이름으로 쌍을 구분
    public bool isEntrance;   // 입구인지 출구인지 구분
}