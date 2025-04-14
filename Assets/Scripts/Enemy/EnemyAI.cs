using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class EnemyAI : MonoBehaviour
{
    protected Dictionary<EnemyStateType, IEnemyState> _states;
    [Header("State")]
    [SerializeField] protected Enums.EnemyStateType _currentStateType;
    private IEnemyState _currentState;

    public float _speed = 2;
    public float _chaseExitDistance = 4;    //chase에서 patrol로 돌아가는 거리
    public float _attackRange = 2;
    [Header("Attack")]
    [SerializeField] private Enums.ATKPatern _curATKPatern;
    [SerializeField]private bool _isAttacking = false;      //현재 공격중인지 판단하는 변수
    [SerializeField]private bool _isAttackFinished = false;        //공격 가능한지 판단하는 변수

    public int _maxPaternNumber = 2;        //실제 패턴 개수 (인덱스 +1 과 같음)



    public Vector2 StartPatrolPoint { get; private set; }
    public Vector2 EndPatrolPoint { get; private set; }
    public Transform Player { get; private set; }
    public Rigidbody2D Rigid { get; private set; }

    public FindTargetPoint _findTargetPoint;  // FindTargetPoint를 참조
    private PatrolPoint _patrolPoint;
    private EnemyATKStats _enemyATKStats;

    public virtual void Initialize()
    {
        CreateState();
        _findTargetPoint = GetComponentInChildren<FindTargetPoint>();
        _patrolPoint = GetComponentInChildren<PatrolPoint>();
        _enemyATKStats = GetComponent<EnemyATKStats>();
        Rigid = GetComponent<Rigidbody2D>();

        _enemyATKStats.OnFinishedAttack += SetFinished;
    }

    public virtual void CreateState()
    {
        // 상태를 생성하는 메서드 (자식 클래스에서 필요하면 구현)
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

    public void EnemyMove(Vector2 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
        //Rigid.MovePosition(Rigid.position + direction * _speed * Time.deltaTime);
        // 회전: 이동 방향으로 회전
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void StartAttack()
    {
        _enemyATKStats.Attack(_findTargetPoint.GetTargetDirection());
    }

    public void SetRandomAttackPatern()
    {
        int randomIndex = Random.Range(0, _maxPaternNumber);  // 1 ~ max-1 랜덤 값0
        _curATKPatern = (ATKPatern)randomIndex;
        _enemyATKStats.SetSkillData(randomIndex);
    }

    public void SetPlayer()
    {
        Player = _findTargetPoint.Player;
    }

    public void SetIsAttacking(bool value)
    {
        _isAttacking = value;
    }

    private void SetFinished(bool value)
    {
        _isAttackFinished = value;
        if (!value)
        {
            SetIsAttacking(value);
        }
    }

    public bool GetIsAttacking()
    {
        return _isAttacking;
    }
    public bool GetIsFinished()
    {
        return _isAttackFinished;
    }

    #region Unity Built-in Fuction



    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        ChangeState(EnemyStateType.Idle); // 기본 상태
    }
    private void OnDisable()
    {
        // _enemyATKStats.OnFinishedAttack -= (value) => _IsAttackFinished = value;
        _enemyATKStats.OnFinishedAttack -= SetFinished;
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
