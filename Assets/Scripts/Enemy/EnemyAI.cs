using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Patrol
    }

    public EnemyState _currentState = EnemyState.Idle;

    public float _speed = 3f;
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
            case EnemyState.Patrol:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.tag == "DetectArea")
            {
                _currentState = EnemyState.Chase;
            }
            else if (other.tag == "AttackArea")
            {
                _currentState = EnemyState.Attack;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _currentState = EnemyState.Idle;
        }
    }

    void ChasePlayer()
    {
        Vector2 _dir = (_player.position - transform.position).normalized;
        transform.Translate(_dir * _speed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        Debug.Log("Attack");
    }
}
