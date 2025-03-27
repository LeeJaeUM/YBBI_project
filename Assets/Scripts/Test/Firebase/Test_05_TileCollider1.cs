using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Test_05_TileCollider1 : TestBase
{
    public Tilemap tilemap; // 타일맵을 할당해야 함
    public TileBase _BaseTile;
    public UnityEngine.Tilemaps.TileData _Data;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogWarning($"지금 닿고있다 {collision.gameObject.name}  ");
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            Debug.Log($"플레이어가 {tilePos} 타일에 닿음!");
        }
    }

    public override void Test2(InputAction.CallbackContext context)
    {
    }
}
