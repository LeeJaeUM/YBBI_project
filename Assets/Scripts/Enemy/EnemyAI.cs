using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class EnemyAI : MonoBehaviour
{
    public Transform Player { get; private set; }
    protected Dictionary<EnemyStateType, IEnemyState> _states;
    protected Enums.EnemyStateType _currentStateType;
    private IEnemyState _currentState;
    public float  _speed;

    public virtual void Initialize()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        //기본 상태 설정 (각 자식 클래스에서 필요하면 변경 가능)
        _states = new Dictionary<EnemyStateType, IEnemyState>
        {
            { EnemyStateType.Idle, new IdleState() },
            { EnemyStateType.Patrol, new PatrolState() },
            { EnemyStateType.Chase, new ChaseState() },
            { EnemyStateType.Attack, new AttackState() }
        };
    }

    public void ChangeState(EnemyStateType newState)
    {
        if (_currentStateType == newState) return; // 같은 상태면 변경 X

        _currentState?.Exit(this);
        _currentStateType = newState;
        _currentState = _states[newState];
        _currentState.Enter(this);
    }

    // 트리거 체크를 별도 함수로 분리 (각 자식 클래스에서 오버라이드 가능)
    protected virtual void CheckTrigger(Collider2D other)
    {
        if (other.CompareTag("DetectArea"))
        {
            ChangeState(EnemyStateType.Chase);
        }
        else if (other.CompareTag("AttackArea"))
        {
            ChangeState(EnemyStateType.Attack);
        }
    }

    void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        ChangeState(EnemyStateType.Idle); // 기본 상태
    }

    void Update()
    {
        _currentState?.Execute(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckTrigger(other);
    }
}
