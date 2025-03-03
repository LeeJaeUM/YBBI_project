using System;
using UnityEngine;

public class TestNet_CharAir : MonoBehaviour
{
    private float _curAir = 10;
    private float _maxAir = 30;
    private float _safeAirZone; // 공기 안정권 (maxAir의 2/3)

    public Action<float> onChangeAir;

    private float _airDecreaseTimer = 0f;
    private float _airDecreaseInterval = 1f; // 1초마다 감소

    private void Awake()
    {
        _safeAirZone = _maxAir * (2f / 3f); // maxAir의 2/3
        _curAir = _safeAirZone + _maxAir * 0.1f;
    }

    public void AddAir(float newValue)
    {
        SetAir(_curAir + newValue);
    }

    public void SetAir(float newValue)
    {
        _curAir = newValue;
        onChangeAir?.Invoke(_curAir);

        // 공기가 maxAir보다 크거나 0보다 작으면 Die() 호출
        if (_curAir >= _maxAir || _curAir <= 0)
        {
            Die();
        }
    }

    public float GetMaxAir()
    {
        return _maxAir;
    }

    private void Update()
    {
        _airDecreaseTimer += Time.deltaTime;
        if (_airDecreaseTimer >= _airDecreaseInterval)
        {
            _airDecreaseTimer = 0f;
            SetAir(Mathf.Max(0, _curAir - 1));
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        // 사망 처리 로직 (예: 게임 오버 화면, 리스폰, 오브젝트 비활성화 등)
        gameObject.SetActive(false);
    }
}
