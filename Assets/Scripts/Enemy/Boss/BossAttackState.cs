using UnityEngine;

public class BossAttackState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is attacking!");

    public void Execute(EnemyAI enemy)
    {
        Debug.Log($"{enemy.name} attacks!");
        if (!enemy.GetIsAttacking())
        {
            enemy.SetIsAttacking(true);
            enemy.SetRandomAttackPatern();
            enemy.StartAttack();
        }
    }

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} stopped attacking.");
}