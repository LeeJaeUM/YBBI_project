using System;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PatrolState : IEnemyState
{
    Vector3 _patrolStartVec = Vector3.zero;
    Vector3 _patrolEndVec = Vector3.zero;
    bool _isAtTarget = false;


    public void Enter(EnemyAI enemy)
    {
        enemy._findTargetPoint.RefreshPlayer();
        Debug.Log($"{enemy.name} is now Patrol");
        _patrolStartVec = enemy.StartPatrolPoint;
        _patrolEndVec = enemy.EndPatrolPoint;
    }

    public void Execute(EnemyAI enemy)
    {
        Vector2 targetPosition = _patrolEndVec;
        if (_isAtTarget)
        {
            targetPosition = _patrolStartVec;
        }
        Vector2 _dir = ((Vector3)targetPosition - enemy.transform.position).normalized;
        enemy._findTargetPoint.SetMoveForward( _dir );

        if(CheckEndPoint(enemy.transform.position, (Vector3)targetPosition))
        {
            _isAtTarget = !_isAtTarget;  // 목표 지점에 도달하면 방향 전환
        }

        enemy.EnemyMove(_dir);

        //순찰 중 플레이어 찾음
        if (enemy._findTargetPoint.CheckPlayerInSight())
        {
            enemy.SetPlayer();
            enemy._findTargetPoint.SetIsChasing();
            enemy.ChangeState(Enums.EnemyStateType.Chase);
        }
    }
    public void Exit(EnemyAI enemy)
    {
    }

    private bool CheckEndPoint(Vector3 start, Vector3 end)
    {
        // 목표 지점에 도달했는지 확인 (제곱 거리 사용)
        float distanceSquared = (start - end).sqrMagnitude;  // 제곱 거리 계산
        float thresholdSquared = 0.1f * 0.1f;  // 임계값의 제곱 (여기서는 0.1f)

        if (distanceSquared < thresholdSquared)  // 제곱된 거리와 임계값 비교
        {
            return true;  // 목표 지점에 도달하면 방향 전환
        }
        return false;
    }


}