using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class EnemyAI : MonoBehaviour
{
    protected Dictionary<EnemyStateType, IEnemyState> _states;
    [SerializeField]protected Enums.EnemyStateType _currentStateType;
    private IEnemyState _currentState;

    public Rigidbody2D Rigid { get; private set; }
    public Transform Player { get; private set; }
    public float _speed = 2;
    public float _attackRange = 2;

    public Vector2 StartPatrolPoint { get; private set; }
    public Vector2 EndPatrolPoint { get; private set; }

    public FindTargetPoint _findTargetPoint;  // FindTargetPoint를 참조
    private PatrolPoint _patrolPoint;

    public virtual void Initialize()
    {
        //기본 상태 설정 (각 자식 클래스에서 필요하면 변경 가능)
        _states = new Dictionary<EnemyStateType, IEnemyState>
        {
            { EnemyStateType.Idle, new IdleState() },
            { EnemyStateType.Patrol, new PatrolState() },
            { EnemyStateType.Chase, new ChaseState() },
            { EnemyStateType.Attack, new AttackState() }
        };

        _findTargetPoint = GetComponent<FindTargetPoint>();
        _patrolPoint = GetComponentInChildren<PatrolPoint>();
        Rigid = GetComponent<Rigidbody2D>();
    }

    public void ChangeState(EnemyStateType newState)
    {
        if (_currentStateType == newState) return; // 같은 상태면 변경 X

        _currentState?.Exit(this);
        _currentStateType = newState;
        _currentState = _states[newState];
        _currentState.Enter(this);
    }

    public void SetPlayer()
    {
        Player = _findTargetPoint.Player;
    }

    public void EnemyMove(Vector2 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
        //Rigid.MovePosition(Rigid.position + direction * _speed * Time.deltaTime);
        // 회전: 이동 방향으로 회전
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #region Unity Built-in Fuction



    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        //_findTargetPoint.OnFindTarget += FindTarget;
        ChangeState(EnemyStateType.Idle); // 기본 상태
    }
    private void OnDisable()
    {
       // _findTargetPoint.OnFindTarget -= FindTarget;
    }

    private void Start()
    {
        StartPatrolPoint = _patrolPoint.GetStartPonint();
        EndPatrolPoint = _patrolPoint.GetEndPonint();
    }
    void Update()
    {
        _currentState?.Execute(this);
    }

    #endregion
}
