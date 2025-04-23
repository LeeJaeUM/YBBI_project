using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestKillAllEnemy : TestBase
{
    [Header("맵그리드(mapGrid)")]
    [SerializeField] GameObject mapGrid;


    public override void Test1(InputAction.CallbackContext context)
    {
        EnemySpawnManager[] enemySpawnManagers = mapGrid.GetComponentsInChildren<EnemySpawnManager>();

        foreach (var manager in enemySpawnManagers)
        {
            List<GameObject> spawnedEnemyList = manager.spawnedEnemyList;
            if (spawnedEnemyList == null || spawnedEnemyList.Count == 0) continue;
            foreach (var enemy in spawnedEnemyList)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }

            spawnedEnemyList.Clear(); // 리스트도 정리하면 깔끔
            StartCoroutine(DelayedCheck(manager));
        }
    }

    private IEnumerator DelayedCheck(EnemySpawnManager manager)
    {
        yield return null; // 1프레임 대기
        manager.CheckAllEnemyDieInRoom();
    }
}
