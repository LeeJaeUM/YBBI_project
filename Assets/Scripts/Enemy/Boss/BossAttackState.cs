using UnityEngine;

public class BossAttackState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is attacking!");

    public void Execute(EnemyAI enemy)
    {
        if (!enemy.GetIsAttacking())
        {
            Debug.Log($"{enemy.name} attacks!");
            enemy.SetIsAttacking(true);
            enemy.SetRandomAttackPatern();
            enemy.StartAttack();
        }
    }

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} stopped attacking.");
}