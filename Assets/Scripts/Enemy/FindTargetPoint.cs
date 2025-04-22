using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class FindTargetPoint : MonoBehaviour
{
    public Transform Player {
        get
        {
            return _player;
        }
        private set
        {
            _player = value;
        } 
    }
    [SerializeField] private Transform _player; 
    public float _visionRange = 4f;  // 시야 거리 : 반지름 (최대 거리)
    public float _visionAngle = 20f;  // 시야 각도 (양옆 20도)
    public Vector2 _moveForward = Vector2.zero;
    public bool _isChasing = false;

    public GameObject _enemyDirObj;
    private List<Transform> _enemiesInRange = new List<Transform>();
    [SerializeField] private Vector2 _directionToNearestEnemy;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isChasing) return;
        if (collision.CompareTag("Player"))
        {
            _enemiesInRange.Add(collision.transform);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _enemiesInRange.Remove(collision.transform);
        }
    }

    private void Update()
    {
        FindNearestEnemy(); //확인용 Update문
    }
    /// <summary>
    /// 범위 안에 들어온 적 중에 가장 가까운 적을 향하는 함수
    /// </summary>
    public void FindNearestEnemy()
    {
        if (_enemiesInRange.Count == 0)
        {
            _directionToNearestEnemy = Vector2.zero;
            return;
        }

        _enemiesInRange.RemoveAll(enemy => enemy == null); // Null 제거
        Debug.Log($"현재 범위내의 Player 개수{_enemiesInRange.Count}");

        _enemiesInRange.Sort((a, b) =>
            Vector2.Distance(transform.position, a.position)
            .CompareTo(Vector2.Distance(transform.position, b.position))
        );

        _player = _enemiesInRange[0];

        // 제일 가까운 적 방향으로 계산
        _directionToNearestEnemy = (_player.position - transform.position).normalized;

    }


    public void SetVisionRange(float visionRange)
    {
        _visionRange = visionRange;
    }

    /// <summary>
    /// 시야 범위 안에 플레이어가 있는지 확인하는 함수 : Patrol 상태때만 update에서 실행
    /// </summary>
    public bool CheckPlayerInSight()
    {
        if(_player == null)
            return false;

        Vector2 directionToPlayer = _player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
        float distanceToPlayer = directionToPlayer.magnitude;  // 적과 플레이어 간의 거리

        // 플레이어가 시야 범위 내에 있는지 확인 (거리)
        if (distanceToPlayer <= _visionRange)
        {
            // 플레이어가 적의 바라보는 방향에 있는지 확인 (각도)
            float angleToPlayer = Vector2.Angle(_moveForward, directionToPlayer);  // transform.up은 적이 바라보는 방향

            if (angleToPlayer <= _visionAngle)
            {
                // 플레이어가 시야 각도 범위 내에 있으면 상태를 변경
                Debug.LogWarning("플레이어 찾음 액션날림");
                //OnFindTarget?.Invoke();
                return true;
            }
        }

        // 디버그용으로 적의 시야 범위를 그려주는 코드
        // 적의 바라보는 방향을 기준으로 원 범위 안에 있는지 확인
        Debug.DrawRay(transform.position, _moveForward * _visionRange, Color.green);  // 적의 앞 방향을 향한 시야 거리
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -_visionAngle) * _moveForward * _visionRange, Color.red);  // 시야 각도 왼쪽
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, _visionAngle) * _moveForward * _visionRange, Color.red);  // 시야 각도 오른쪽

        return false;
    }

    /// <summary>
    /// 공격가능거리면 true 아니면 false
    /// </summary>
    /// <param name="range">공격거리</param>
    /// <returns></returns>
    public bool CheckTargetInRange(float range)
    {
        Vector2 directionToPlayer = _player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
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
        Vector2 directionToPlayer = _player.position - transform.position;  // 적에서 플레이어로 향하는 벡터
        float distanceToPlayer = directionToPlayer.magnitude;  // 적과 플레이어 간의 거리
        
        return distanceToPlayer;    
    }

    public Vector3 GetTargetDirection()
    {
        return (_player.position - transform.position).normalized;  // 적에서 플레이어로 향하는 벡터
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
        _player = null;  
        _isChasing = false;
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (_isChasing) return;
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("플레이어 세팅");
    //       Player = other.transform;
    //    }
    //}

}
