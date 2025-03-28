using System;
using UnityEngine;
using static Enums;

public class FindTargetPoint : MonoBehaviour
{
    public Transform Player { get; private set; }
    public float visionRange = 4f;  // 시야 거리 : 반지름 (최대 거리)
    public float visionAngle = 20f;  // 시야 각도 (양옆 20도)
    public Vector2 _moveForward = Vector2.zero;
    public bool _isChasing = false;

    //public Action OnFindTarget;

    /// <summary>
    /// 시야 범위 안에 플레이어가 있는지 확인하는 함수 : Patrol 상태때만 update에서 실행
    /// </summary>
    public bool CheckPlayerInSight()
    {
        if(Player == null)
            return false;

        Vector2 directionToPlayer = Player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
        float distanceToPlayer = directionToPlayer.magnitude;  // 적과 플레이어 간의 거리

        // 플레이어가 시야 범위 내에 있는지 확인 (거리)
        if (distanceToPlayer <= visionRange)
        {
            // 플레이어가 적의 바라보는 방향에 있는지 확인 (각도)
            float angleToPlayer = Vector2.Angle(_moveForward, directionToPlayer);  // transform.up은 적이 바라보는 방향

            if (angleToPlayer <= visionAngle)
            {
                // 플레이어가 시야 각도 범위 내에 있으면 상태를 변경
                Debug.LogWarning("플레이어 찾음 액션날림");
                //OnFindTarget?.Invoke();
                return true;
            }
        }

        // 디버그용으로 적의 시야 범위를 그려주는 코드
        // 적의 바라보는 방향을 기준으로 원 범위 안에 있는지 확인
        Debug.DrawRay(transform.position, _moveForward * visionRange, Color.green);  // 적의 앞 방향을 향한 시야 거리
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -visionAngle) * _moveForward * visionRange, Color.red);  // 시야 각도 왼쪽
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, visionAngle) * _moveForward * visionRange, Color.red);  // 시야 각도 오른쪽

        return false;
    }

    /// <summary>
    /// 공격가능거리면 true 아니면 false
    /// </summary>
    /// <param name="range">공격거리</param>
    /// <returns></returns>
    public bool CheckTargetInRange(float range)
    {
        Vector2 directionToPlayer = Player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
        float distanceToPlayer = directionToPlayer.magnitude;  // 적과 플레이어 간의 거리

        // 플레이어가 공격가능거리 내에 있는지 확인 (거리)
        if (distanceToPlayer <= range)
        {
            return true;
        }
            return false;
    }

    /// <summary>
    /// 대상 까지의 거리를 리턴
    /// </summary>
    public float GetDirectionToTarget()
    {
        Vector2 directionToPlayer = Player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
        float distanceToPlayer = directionToPlayer.magnitude;  // 적과 플레이어 간의 거리
        
        return distanceToPlayer;    
    }

    public Vector3 GetTargetDirection()
    {
        return Player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
    }

    public void SetMoveForward(Vector2 forward)
    {
        _moveForward = forward;
    }

    public void SetIsChasing()
    {
        _isChasing = true;
    }
    public void RefreshPlayer()
    {
        Player = null;  
        _isChasing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isChasing) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 세팅");
           Player = other.transform;
        }
    }
}
