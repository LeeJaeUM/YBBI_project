using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    protected float _curAir = 10;
    protected float _maxAir = 30;
    protected float _safeAirZone; // 공기 안정권 (maxAir의 2/3)
    public float _minusAirPersec = 1;

    public Action<float> onChangeAir;
    public Action OnDie;

    private float _airDecreaseTimer = 0f;
    private float _airDecreaseInterval = 1f; // 1초마다 감소


    public void SetAir(float newValue)
    {
        _curAir = newValue;
        onChangeAir?.Invoke(_curAir);

        if (_curAir >= _maxAir || _curAir <= 0)
        {
            Die();
        }
    }

    public void AddAir(float addValue)
    {
        if (addValue < 0)
        {
            HitDamage();        //데미지를 입으면 함수 호출
        }
        SetAir(_curAir + addValue);
    }

    public float GetAir()
    {
        return _curAir; 
    }

    public float GetMaxAir()
    {
        return _maxAir;
    }

    protected virtual void Die()
    {
        OnDie?.Invoke();
    }

    /// <summary>
    /// 애니메이션용 데미지 입을 때 호출되는 함수
    /// </summary>
    protected virtual void HitDamage()
    {

    }

    //public bool IsInSafeZone()
    //{
    //    return _curAir >= _safeAirZone;
    //}

    protected virtual void Awake()
    {
        _safeAirZone = _maxAir * (2f / 3f);
        _curAir = _maxAir;
    }


    protected virtual void Update()
    {
        _airDecreaseTimer += Time.deltaTime;
        if (_minusAirPersec != 0 && _airDecreaseTimer >= _airDecreaseInterval)
        {
            _airDecreaseTimer = 0f;
            SetAir(Mathf.Max(0, _curAir - _minusAirPersec));
        }
    }
}
