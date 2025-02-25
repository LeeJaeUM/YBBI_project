using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState 
    { 
        Idle, 
        Chase, 
        Attack 
    }

    public EnemyState _currentState = EnemyState.Idle;
    public Transform _player;
    public float _chaseDist = 5f;
    public float _attackDist = 1f;
    public float _speed = 4f;

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        switch (_currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer < _chaseDist)
                {
                    _currentState = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                if (distanceToPlayer > _chaseDist)
                {
                    _currentState = EnemyState.Idle;
                }
                else if (distanceToPlayer < _attackDist)
                {
                    _currentState = EnemyState.Attack;
                }
                else
                {
                    Vector2 direction = (_player.position - transform.position).normalized;
                    transform.Translate(direction * _speed * Time.deltaTime);
                }
                break;
            case EnemyState.Attack:
                if (distanceToPlayer > _attackDist)
                {
                    _currentState = EnemyState.Chase;
                }
                else
                {
                    
                }
                break;
        }
    }
}
