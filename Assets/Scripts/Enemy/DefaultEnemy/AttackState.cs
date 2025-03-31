using UnityEngine;

public class AttackState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is attacking!");

    public void Execute(EnemyAI enemy)
    {
        Debug.Log($"{enemy.name} attacks!");
        if(!enemy.GetIsAttacking())
        {
            enemy.SetIsAttacking(true);
            enemy.SetRandomAttackPatern();
            enemy.StartAttack();
        }
        else
        {
            if(enemy.GetIsFinished())
            {
                //공격중이 아닐때 공격 거리를 벗어나면 chase로 변경
                if (!enemy._findTargetPoint.CheckTargetInRange(enemy._attackRange))
                {
                    enemy.ChangeState(Enums.EnemyStateType.Chase);
                }
            }
        }
    }

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} stopped attacking.");
}