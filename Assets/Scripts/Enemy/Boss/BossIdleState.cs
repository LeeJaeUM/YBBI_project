using UnityEngine;

public class BossIdleState : IEnemyState
{
    private float _idleTime = 4f;  // 대기 시간 (3초)
private float _timeSpentInState = 0f;  // 현재 상태에서 경과된 시간

public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is now Idle");

public void Execute(EnemyAI enemy)
{
    // 경과된 시간 업데이트
    _timeSpentInState += Time.deltaTime;

    // _idleTime이 경과하면 상태 변경
    if (_timeSpentInState >= _idleTime)
    {
        enemy.ChangeState(Enums.EnemyStateType.Attack);
    }
}

public void Exit(EnemyAI enemy)
{
    Debug.Log($"{enemy.name} is leaving Idle state");
}
}
