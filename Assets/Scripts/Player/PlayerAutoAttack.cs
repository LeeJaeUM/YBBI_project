using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour
{
    public float _attackDamage = 10f;

    private List<Transform> _enemiesInRange = new List<Transform>();

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

    public void AttackClosestEnemy()
    {
        if (_enemiesInRange.Count == 0)
        {
            return;
        }

        Transform _closestEnemy = null;
        float _closestDist = Mathf.Infinity;

        foreach (Transform _enemy in _enemiesInRange)
        {
            float _distance = Vector2.Distance(transform.position, _enemy.position);

            if (_distance < _closestDist)
            {
                _closestDist = _distance;
                _closestEnemy = _enemy;
            }
        }

        if (_closestEnemy != null) 
        {
            Debug.Log("Attack Closest");
        }
    }
}
