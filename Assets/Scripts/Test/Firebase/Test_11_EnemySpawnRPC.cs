using UnityEngine;
using UnityEngine.InputSystem;

public class Test_11_EnemySpawnRPC : TestBase
{
    public RPC_EnemySpawn enemySpawn;
    public override void Test1(InputAction.CallbackContext context)
    {
        enemySpawn.SpawnEnemy();
    }
}
