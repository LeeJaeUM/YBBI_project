using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/TileMapTelepoter")]
public class TelepoterTile : Tile
{
    public string teleportID; // "A", "B" 같은 이름으로 쌍을 구분
}