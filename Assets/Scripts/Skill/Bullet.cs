using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    public bool _isPlayers = true;
    public float _lifeTime = 2;
    public float _damage = 5;
    public float _speed = 6f; // 총알 속도
    public Vector2 _arrowVec = Vector2.right;
    public bool _canMove = true; //움직임 여부, 움직이지 않는 투사체라면 닿아도 사라지지 않음

    public bool _canTrigger = true;
    public bool _isLingering = false; //지속형 스킬인지 여부
    public bool _isHoming = false; //추적형 스킬인지 여부

    private float _tickTimer = 0f;
    [SerializeField] private float _tickInterval = 0.35f;

    private float _homingRadius = 20f;

    Enums.BulletType _bulletType = Enums.BulletType.NONE;

    private LaserWarningVisualizer _laserWarningVisualizer;
    public SpriteRenderer _spriteRenderer;  
    private BoxCollider2D _boxCollider2D; //충돌 콜라이더
    private BulletAnimator _bulletAnimator;
    [SerializeField]private Transform _target; //추적형 스킬의 타겟

    private void Initialize()
    {
        transform.localScale = Vector3.one;
        _spriteRenderer.transform.localPosition = Vector3.zero; //스프라이트 위치 초기화
        _spriteRenderer.transform.localScale = Vector3.one;
        _boxCollider2D.size = Vector3.one;

        _canTrigger = true;
        _canMove = true;
        _isLingering = false;
        _isHoming = false;
        _target = null;
        StartCoroutine(LifeTimeCor()); //테스트
    }

    public void SetArrowVector(Vector2 value)
    {
        _arrowVec = value;
    }

    private void ResetBulletSize()
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        _isLingering = false;
    }

    private void IsLingeringBullet(bool isTrue)
    {
        if(isTrue)
        {
            _isLingering = true;
        }
        else
        {
            _isLingering = false;
        }
    }

    /// <summary>
    /// 직선으로만 나아가는 이동이라면 true
    /// </summary>
    /// <param name="isTrue"></param>
    private void IsMoveable(bool isTrue)
    {
        if (isTrue)
        {
            _canMove = true;
        }
        else
        {
            _canMove = false;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="radius">공격범위</param>
    /// <param name="activeTime">활성시간</param>
    /// <param name="isMove">움직임 여부</param>
    public void SetData(float damage, float radius, float width, float length, float activeTime, float moveSpeed, Enums.BulletType bulletType = Enums.BulletType.Normal)
    {
        ResetBulletSize();
        _damage = damage;
        transform.localScale = Vector3.one * radius;
        _boxCollider2D.offset = Vector3.zero;
        _lifeTime = activeTime;
        _speed = moveSpeed;
        _bulletType = bulletType;

        if(damage > 0)                              //damage가 0보다 크면 데미지
            _spriteRenderer.color = Color.red;
        else if(damage <= 0)                        //0보다 작으면 힐
            _spriteRenderer.color = Color.green;

        _bulletAnimator.SetAnimator(_bulletType);      //애니메이터 컨트롤러 설정

        StopAllCoroutines(); //기존 코루틴 정지
        switch(_bulletType)
        {
            case Enums.BulletType.Laser:
                if (_laserWarningVisualizer != null)
                {
                    StartCoroutine(TriggerOff_Laser(moveSpeed));

                    IsMoveable(false); //이동 정지
                    IsLingeringBullet(true); //지속형 스킬로 설정
                    
                    _laserWarningVisualizer.ShowLaserWarning(transform.position, _arrowVec, width, length, moveSpeed);
                    
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, _arrowVec); //회전
                    _spriteRenderer.transform.localScale = new Vector3(width, length, 1f);
                    _spriteRenderer.transform.localPosition = new Vector3(0, length / 2f, 0); // 중심에서 위로
                }
                break;

            case Enums.BulletType.Homing:
                _isHoming = true; //추적형 스킬로 설정
                if(!_isPlayers)
                    FindTarget("Player"); //플레이어 찾기
                else
                    FindTarget("Enemy"); //적 찾기 


                break;

            case Enums.BulletType.Bomb:

                IsMoveable(false); //이동 정지
                IsLingeringBullet(true); //지속형 스킬로 설정
                // 방향 벡터 기반 도착 지점 계산
                Vector2 destination = (Vector2)transform.position + _arrowVec.normalized * length;
                Vector3 spawnPosition = transform.position; // 발사 위치

                transform.position = destination;
                _spriteRenderer.transform.localPosition = -_arrowVec.normalized * length / 2f;

                float readyTime = _speed; // 폭발까지의 시간
                StartCoroutine(TriggerOff_Bomb(readyTime)); //트리거 정지

                // 이동 연출 시작 (spawnPosition은 발사 시작 위치, bullet은 도착 위치)
                StartCoroutine(MoveBombWarningFromSpawn(spawnPosition, readyTime));
                // 폭발 범위 표시
                _laserWarningVisualizer.ShowBombWarning(readyTime);

                // 이동 코루틴 시작
                StartCoroutine(MoveAndExplode(destination, readyTime));
                break;
            default:
                break;
        }

        //lifeTime 설정 후 코루틴 시작
        StartCoroutine(LifeTimeCor());
    }

    #region Bomb

    /// <summary>
    /// 폭탄 스프라이를 발사 위치에서 목표 위치로 이동시키는 코루틴
    /// </summary>
    /// <param name="spawnPosition"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator MoveBombWarningFromSpawn(Vector3 spawnPosition, float duration)
    {
        float elapsed = 0f;

        // 시작 위치는 총알 발사 위치
        Vector3 startPos = spawnPosition;
        // 도착 위치는 bullet 오브젝트 위치 (부모)
        Vector3 endPos = transform.position;

        _spriteRenderer.transform.position = startPos;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            _spriteRenderer.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _spriteRenderer.transform.position = endPos;
    }
    private void Explode()
    {
        // 범위 내 적 감지 및 피해
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);
        foreach (var hit in hits)
        {
            var health = hit.GetComponent<UnitHealth>();
            if (health != null)
            {
                health.DamageAir(_damage);  //일괄적으로 모두에게 데미지
            }
        }

        // 폭발 이펙트 재생
        //if (_explosionEffect != null)
        //{
        //    Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        //}
    }

    private IEnumerator MoveAndExplode(Vector2 targetPos, float moveDuration)
    {
        Vector2 startPos = transform.position;
        float elapsed = 0f;

        // 충돌 제거 (충돌 없음)
        //_boxCollider2D.enabled = false;

        // 이동
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 도착 후 폭발
        Explode();
        _bulletAnimator.PlayEndAnimation();

    }
    #endregion

    #region Homming
    private void FindTarget(string targetTagName)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _homingRadius);

        Transform closestPlayer = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTagName))
            {
                float distanceSqr = (hit.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestPlayer = hit.transform;
                }
            }
        }

        if (closestPlayer != null)
        {
            _target = closestPlayer;
        }
    }
    #endregion
    IEnumerator LifeTimeCor()
    {
        yield return new WaitForSeconds(_lifeTime);
        StopBullet(false); 
    }
    IEnumerator TriggerOff_Laser(float time)
    {
        _canTrigger = false;
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(time);
        _canTrigger = true;
        _spriteRenderer.enabled = true;
        _bulletAnimator.PlayStartAnimation();
    }
    IEnumerator TriggerOff_Bomb(float time)
    {
        _bulletAnimator.PlayStartAnimation();
        _canTrigger = false;
        yield return new WaitForSeconds(time);
        _canTrigger = true;
    }

    IEnumerator EndAnimation(bool isTriggered)
    {
        _canTrigger = false;
        if(isTriggered)
        {
            _bulletAnimator.PlayEndAnimation();
            _speed = 0;
        }

        yield return new WaitForSeconds(0.8f);
        BulletPool.Instance.ReturnBullet(gameObject); //풀에 반납
    }

    private void StopBullet(bool isTriggered)
    {
        StopAllCoroutines();
        StartCoroutine(EndAnimation(isTriggered));
    }
    private void TriggerBullet(Collider2D collision)
    {
        Debug.Log($"{collision.gameObject.name} 이 닿음");
        UnitHealth unitHealth = collision.GetComponent<UnitHealth>();
        unitHealth.DamageAir(_damage);

        if (_canMove)
            StopBullet(true);
    }

    private void CheckEnemyOrPlayer(Collider2D collision)
    {
        if (_isPlayers)
        {
            if (collision.CompareTag("Enemy"))
            {
                TriggerBullet(collision); //적에게 닿았을 때
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                TriggerBullet(collision); //플레이어에게 닿았을 때
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_canTrigger && _isLingering)
        {

            if (_tickTimer >= _tickInterval)
            {
                CheckEnemyOrPlayer(collision);
                _tickTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canTrigger && !_isLingering)
        {
            CheckEnemyOrPlayer(collision);
        }

    }

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        _spriteRenderer = child.GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        _bulletAnimator = GetComponentInChildren<BulletAnimator>();
        _laserWarningVisualizer = GetComponentInChildren<LaserWarningVisualizer>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    //private void OnDisable()
    //{
    //    StopAllCoroutines();
    //    _canMove = true;
    //    _canTrigger = true;
    //    _spriteRenderer.enabled = true;
    //    _laserWarningVisualizer.HideWarning();
    //}

    private void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_isHoming)
        {
            if (_target != null)
            {
                Vector2 direction = (_target.position - transform.position).normalized;
                SetArrowVector(direction);
            }
        }

        if (_canMove)
            transform.Translate(_arrowVec * _speed * Time.deltaTime);


    }
}
