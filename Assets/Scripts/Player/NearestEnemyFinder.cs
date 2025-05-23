using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NearestEnemyFinder : MonoBehaviour
{
    public GameObject _enemyDirObj;
    private List<Transform> _enemiesInRange = new List<Transform>();
    [SerializeField] private Vector2 _directionToNearestEnemy;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemiesInRange.Add(collision.transform);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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

        _enemiesInRange.Sort((a, b) =>
            Vector2.Distance(transform.position, a.position)
            .CompareTo(Vector2.Distance(transform.position, b.position))
        );

        Transform _closestEnemy = _enemiesInRange[0];

        // 제일 가까운 적 방향으로 계산
        _directionToNearestEnemy = (_closestEnemy.position - transform.position).normalized;

        // 적 방향으로 회전
        float _angle = Mathf.Atan2(_directionToNearestEnemy.y, _directionToNearestEnemy.x) * Mathf.Rad2Deg;
        _enemyDirObj.transform.rotation = Quaternion.Euler(0, 0, _angle);
    }

    /// <summary>
    /// 제일 가까운 적의 방향을 리턴하는 함수
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirectionToNearestEnemy()
    {
        return _directionToNearestEnemy;
    }
}
