using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool _isPlayers = true;
    public float _lifeTime = 2;
    public bool _isPlusAir = false;     //데미지가 +인지 -인지 판단하는 변수
    public float _damageMul = 1;        //데미지가 +인지 -인지 정하는 1/-1 곱하기용 변수
    public float _damage = 5;
    public float _speed = 6f; // 총알 속도
    public Vector2 _arrowVec = Vector2.right;

    public bool _canTrigger = false;

    public void SetArrowVector(Vector2 value)
    {
        _arrowVec = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="radius">공격범위</param>
    /// <param name="activeTime">활성시간</param>
    /// <param name="isMove">움직임 여부</param>
    public void SetData(float damage, float radius, float activeTime, float moveSpeed)
    {
        _damage = damage;
        transform.localScale = Vector3.one * radius;
        _lifeTime = activeTime;
        _speed = moveSpeed;

        //lifeTime 설정 후 코루틴 시작
        StartCoroutine(LifeTimeCor());
    }

    IEnumerator LifeTimeCor()
    {
        yield return new WaitForSeconds(_lifeTime);
        gameObject.SetActive(false);
    }


    private void CheckBullet(UnitHealth unitHealth)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canTrigger)
        {
            if (_isPlayers)
            {
                if (collision.CompareTag("Enemy"))
                {
                    // Debug.Log($"{collision.gameObject.name} 이 닿음");
                    UnitHealth unitHealth = collision.GetComponent<UnitHealth>();
                    unitHealth.AddAir(_damage * _damageMul);
                        // Debug.Log("데미지 총알");
                    StopAllCoroutines();
                    gameObject.SetActive(false); //움직임 종료
                }
            }
            else
            {
                if (collision.CompareTag("Player"))
                {
                    // Debug.Log($"{collision.gameObject.name} 이 닿음");
                    UnitHealth unitHealth = collision.GetComponent<UnitHealth>();
                    unitHealth.AddAir(_damage * _damageMul);
                    StopAllCoroutines();
                    gameObject.SetActive(false); //움직임 종료
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
        if (!_isPlusAir)
            _damageMul = -1;

    }
    private void Update()
    {
        transform.Translate(_arrowVec * _speed * Time.deltaTime);
    }
}
