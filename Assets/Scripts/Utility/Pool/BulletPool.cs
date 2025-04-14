using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _initialSize = 20;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < _initialSize; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform);
            bullet.SetActive(false);
            _pool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (_pool.Count > 0)
        {
            GameObject bullet = _pool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            // 풀 부족 시 추가 생성
            GameObject bullet = Instantiate(_bulletPrefab);
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        _pool.Enqueue(bullet);
    }
}
