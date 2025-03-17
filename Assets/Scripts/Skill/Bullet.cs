using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float _lifeTime = 4;
    public bool _isPlusAir = false;
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
    }

    IEnumerator LifeTimeCor()
    {
        yield return new WaitForSeconds(_lifeTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canTrigger)
        {
            if (collision.CompareTag("Enemy"))
            {
               // Debug.Log($"{collision.gameObject.name} 이 닿음");
                if (!_isPlusAir)
                {
                   // Debug.Log("데미지 총알");
                }
                else
                {
                   // Debug.Log("회복 총알");
                }
                StopAllCoroutines();
                gameObject.SetActive(false); //움직임 종료
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _canTrigger = true;
    }

    private void OnEnable()
    {
        StartCoroutine(LifeTimeCor());
    }
    private void Update()
    {
        transform.Translate(_arrowVec * _speed * Time.deltaTime);
    }
}
