using UnityEngine;

public class IdleState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is now Idle");

    public void Execute(EnemyAI enemy) { }  // 아무 행동 없음

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} is leaving Idle state");
}
