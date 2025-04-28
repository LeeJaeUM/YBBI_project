using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class EnemyAI : MonoBehaviour
{
    public bool TEST_isPatternSet = false; //패턴 세팅 테스트용
    public int TEST_patternNum = 0; //패턴 세팅 테스트용
    protected Dictionary<EnemyStateType, IEnemyState> _states;
    [Header("State")]
    [SerializeField] protected Enums.EnemyStateType _currentStateType;
    private IEnemyState _currentState;

    public float _speed = 2;
    public float _chaseExitDistance = 4;    //chase에서 patrol로 돌아가는 거리
    public float _attackRange = 2;
    [Header("Attack")]
    [SerializeField] private Enums.ATKPatern _curATKPatern;
    [SerializeField] private bool _isAttacking = false;      //현재 공격중인지 판단하는 변수
    [SerializeField] private bool _isAttackFinished = false;        //공격 가능한지 판단하는 변수

    public int _maxPaternNumber = 2;        //실제 패턴 개수 (인덱스 +1 과 같음)



    public Vector2 StartPatrolPoint { get; private set; }
    public Vector2 EndPatrolPoint { get; private set; }
    public Transform Player { get; private set; }
    public Rigidbody2D Rigid { get; private set; }

    public FindTargetPoint _findTargetPoint;  // FindTargetPoint를 참조
    private PatrolPoint _patrolPoint;
    private EnemyATKStats _enemyATKStats;
    private EnemyAnimator _enemyAniamtor;

    public virtual void Initialize()
    {
        CreateState();
        _findTargetPoint = GetComponentInChildren<FindTargetPoint>();
        _patrolPoint = GetComponentInChildren<PatrolPoint>();
        _enemyATKStats = GetComponent<EnemyATKStats>();
        Rigid = GetComponent<Rigidbody2D>();
        _enemyAniamtor = GetComponentInChildren<EnemyAnimator>();

        _enemyATKStats.OnFinishedAttack += SetAttackFinished;
        _enemyATKStats.OnMoveSkill += StartRandomMove; //이동 스킬 시작
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
        Vector3 vector3 = _findTargetPoint.GetTargetDirection();
        _enemyAniamtor.UpdateMoveVisual(vector3);        //적의 바라보는 방향에 따라 애니메이션을 업데이트
        _enemyAniamtor.PlayAttackAnimation();

        _enemyATKStats.Attack(vector3);
    }

    public void SetRandomAttackPatern()
    {
        int randomIndex = Random.Range(0, _maxPaternNumber);  // 1 ~ max-1 랜덤 값0
#if UNITY_EDITOR
        if (TEST_isPatternSet) //에디터에서만 사용
        {
            randomIndex = TEST_patternNum; //에디터에서만 사용
        }
#endif
        _curATKPatern = (ATKPatern)randomIndex;
        _enemyATKStats.SetPaaternNum(randomIndex);
    }

    public bool GetIsAttacking()
    {
        return _isAttacking;
    }
    public void SetIsAttacking(bool value)
    {
        _isAttacking = value;
    }

    public bool GetIsFinished()
    {
        return _isAttackFinished;
    }
    private void SetAttackFinished(bool value)
    {
        _isAttackFinished = value;
        if (!value)                 //공격이 완전히 종료되면 
        {
            SetIsAttacking(value);  //isAttacking을 false로 변경
        }
    }

    public void SetPlayer()
    {
        Player = _findTargetPoint.Player;
    }

    public void StartRandomMove(float moveDistance, float moveSpeed, float activeTime)
    {
        StartCoroutine(RandomMoveCoroutine(moveDistance, moveSpeed, activeTime));
    }

    private IEnumerator RandomMoveCoroutine(float moveDistance, float moveSpeed, float activeTime)
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            (Vector3.up + Vector3.right).normalized,    // 우상향
            (Vector3.up + Vector3.left).normalized,     // 좌상향
            (Vector3.down + Vector3.right).normalized,  // 우하향
            (Vector3.down + Vector3.left).normalized    // 좌하향
        };

        Vector3 selectedDir = directions[Random.Range(0, directions.Length)];

        // 목표 위치 계산
        Vector3 targetPos = CalculateTargetPosition(transform.position, selectedDir, moveDistance);

        float elapsed = 0f;
        while (elapsed < activeTime && Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // 위치 보정
    }
    private Vector3 CalculateTargetPosition(Vector3 startPos, Vector3 direction, float moveDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, moveDistance, LayerMask.GetMask("Map"));

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            // 벽에 맞았으면 충돌 지점까지 거리 - 2 만큼만 이동
            float distanceToWall = hit.distance - 2;
            distanceToWall = Mathf.Max(0f, distanceToWall); // 0보다 작으면 0으로 보정
            return startPos + direction * distanceToWall;
        }
        else
        {
            // 벽이 없으면 그냥 원래 거리만큼 이동
            return startPos + direction * moveDistance;
        }
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
        _enemyATKStats.OnFinishedAttack -= SetAttackFinished;
        _enemyATKStats.OnMoveSkill -= StartRandomMove; //이동 스킬
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
