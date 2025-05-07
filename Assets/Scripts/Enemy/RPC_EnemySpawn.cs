using UnityEngine;
using Unity.Netcode;

public class RPC_EnemySpawn : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
           // SpawnEnemyClientRpc();
        }
    }


    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
    }

}
