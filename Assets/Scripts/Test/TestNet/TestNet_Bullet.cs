using System.Collections;
using UnityEngine;

public class TestNet_Bullet : MonoBehaviour
{
    public float _lifeTime = 4;
    public bool _isPlusAir = true;
    public float _airPoint = 5;
    public float _speed = 3f; // 총알 속도
    public Vector2 _arrowVec = Vector2.right;

    public void SetArrowVector(Vector2 value)
    {
        _arrowVec = value;
    }

    IEnumerator LifeTimeCor()
    {
        yield return new WaitForSeconds(_lifeTime);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("닿음");
        TestNet_CharAir charAir = collision.GetComponent<TestNet_CharAir>();
        if (charAir != null)
        {
            if (_isPlusAir)
            {
                Debug.Log("공기 줌");
                charAir.AddAir(_airPoint);
            }
            else
            {
                charAir.AddAir(_airPoint * -1);
            }
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
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
