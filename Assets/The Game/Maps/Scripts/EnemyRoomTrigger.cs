using Unity.Netcode;
using UnityEngine;

public class EnemyRoomTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawnManager spawnManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return; // Netcode 사용 시 필수
        if (!other.CompareTag("Player")) return;

        Debug.Log("[EnemyRoomTrigger] 플레이어가 방 입장함 → 전투 시작");
        spawnManager?.RequestEnterFight();
    }
}
