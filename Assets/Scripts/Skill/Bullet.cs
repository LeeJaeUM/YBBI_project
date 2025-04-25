using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public bool _isPlayers = true;
    public float _lifeTime = 2;
    public bool _isPlusAir = false;     //데미지가 +인지 -인지 판단하는 변수
    public float _damageMul = 1;        //데미지가 +인지 -인지 정하는 1/-1 곱하기용 변수
    public float _damage = 5;
    public float _speed = 6f; // 총알 속도
    public Vector2 _arrowVec = Vector2.right;
    public bool _canMove = true; //움직임 여부, 움직이지 않는 투사체라면 닿아도 사라지지 않음

    public bool _canTrigger = true;

    Enums.BulletType _bulletType = Enums.BulletType.NONE;

    private LaserWarningVisualizer _laserWarningVisualizer;
    public SpriteRenderer _spriteRenderer;  
    private BoxCollider2D _boxCollider2D; //충돌 콜라이더

    public void SetArrowVector(Vector2 value)
    {
        _arrowVec = value;
    }

    private void ResetBulletSize()
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
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
        _lifeTime = activeTime;
        _speed = moveSpeed;
        _bulletType = bulletType;

        StopAllCoroutines(); //기존 코루틴 정지
        switch(_bulletType)
        {
            case Enums.BulletType.Laser:
                _laserWarningVisualizer = GetComponentInChildren<LaserWarningVisualizer>();
                if (_laserWarningVisualizer != null)
                {
                Debug.Log("찾음레이저");
                    StartCoroutine(TriggerOff(moveSpeed));
                    _canMove = false; //이동 정지
                    _laserWarningVisualizer.ShowLaserWarning(transform.position, _arrowVec, width, length, moveSpeed);
                    
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, _arrowVec); //회전
                    _spriteRenderer.transform.localScale = new Vector3(width, length, 1f);
                    _spriteRenderer.transform.localPosition = new Vector3(0, length / 2f, 0); // 중심에서 위로
                }
                break;
            case Enums.BulletType.Normal:
                break;
            default:
                break;
        }

        //lifeTime 설정 후 코루틴 시작
        StartCoroutine(LifeTimeCor());
    }

    IEnumerator LifeTimeCor()
    {
        yield return new WaitForSeconds(_lifeTime);
        StopBullet(); 
    }
    IEnumerator TriggerOff(float time)
    {
        _canTrigger = false;
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(time);
        _canTrigger = true;
        _spriteRenderer.enabled = true;
    }

    private void TriggerBullet(Collider2D collision)
    {
        Debug.Log($"{collision.gameObject.name} 이 닿음");
        UnitHealth unitHealth = collision.GetComponent<UnitHealth>();
        unitHealth.AddAir(_damage * _damageMul);

        if(_canMove)        
            StopBullet();
    }

    private void StopBullet()
    {
        StopAllCoroutines();
        BulletPool.Instance.ReturnBullet(gameObject); //풀에 반납
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canTrigger)
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

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _canTrigger = true;
    }
    private void Awake()
    {
        Transform child = transform.GetChild(0);
        _spriteRenderer = child.GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    private void OnEnable()
    {
        if (!_isPlusAir)
            _damageMul = -1;

        transform.localScale = Vector3.one;
        _spriteRenderer.transform.localPosition = Vector3.zero; //스프라이트 위치 초기화
        _spriteRenderer.transform.localScale = Vector3.one;
        _boxCollider2D.size = Vector3.one;

        _canMove = true;
        StartCoroutine(LifeTimeCor()); //테스트
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
        if(_canMove)
            transform.Translate(_arrowVec * _speed * Time.deltaTime);
    }
}
