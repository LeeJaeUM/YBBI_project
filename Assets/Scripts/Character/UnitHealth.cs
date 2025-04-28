using System;
using System.Collections;
using UnityEngine;
using static Enums;


public class UnitHealth : MonoBehaviour
{

    public DebuffFlags _currentDebuffs = DebuffFlags.None;

    [SerializeField]protected float _curAir = 10;
    [SerializeField]protected float _maxAir = 100;
    protected float _safeAirZone; // 공기 안정권 (maxAir의 2/3)
    [SerializeField]private float _baseMinusAirPerSec = 1f;
    public float _minusAirPerSec = 1;   // 감소하는 공기량 (데미지량)
    [SerializeField]private float _baseAddAirPerSec = 0f;
    public float _addAirPerSec = 0;     // 증가하는 공기량 (힐량)
    public float _curAirPerSec = 0; // 현재 공기량 (힐량 + 데미지량)

    public Action<float> onChangeAir;
    public Action OnDamage;
    public Action OnDie;

    private float _airDecreaseTimer = 0f;
    private float _baseAirDecreaseInterval = 1f; 
    private float _airDecreaseInterval = 1f; // 1초마다 감소

    #region Debuff

    public void StartDebuff(float duration, DebuffFlags debuff)
    {
        // 코루틴 시작
        StartCoroutine(DebuffCoroutine(duration, debuff));
    }

    IEnumerator DebuffCoroutine(float duration, DebuffFlags debuff)
    {
        ApplyDebuff(debuff);
        yield return new WaitForSeconds(duration);
        RemoveDebuff(debuff);
    }

    /// <summary>
    /// 디버프 적용
    /// </summary>
    private void ApplyDebuff(DebuffFlags debuff)
    {        
        // 이미 있는 디버프면 무시
        if (HasDebuff(debuff))
            return;

        _currentDebuffs |= debuff;
        RecalculateAirPerSec();
    }

    /// <summary>
    /// 디버프 제거
    /// </summary>
    private void RemoveDebuff(DebuffFlags debuff)
    {
        _currentDebuffs &= ~debuff;
        RecalculateAirPerSec();
    }

    /// <summary>
    /// 모든 디버프 제거 (정화)
    /// </summary>
    public void Cleanse()
    {
        _currentDebuffs = DebuffFlags.None;
        RecalculateAirPerSec();
    }

    /// <summary>
    /// 해당 디버프가 존재하는지 확인
    /// </summary>
    public bool HasDebuff(DebuffFlags debuff)
    {
        return (_currentDebuffs & debuff) != 0;
    }

    /// <summary>
    /// 현재 모든 디버프 가져오기 (디버깅용)
    /// </summary>
    public DebuffFlags GetCurrentDebuffs()
    {
        return _currentDebuffs;
    }

    private void RecalculateAirPerSec()
    {
        // 시작은 기본값
        _minusAirPerSec = _baseMinusAirPerSec;
        _addAirPerSec = _baseAddAirPerSec;
        _airDecreaseInterval = _baseAirDecreaseInterval;

        // 감소율 2배 디버프: *2
        if (HasDebuff(DebuffFlags.DecreaseRate2x))
            _minusAirPerSec *= 2f;

        // 증가율 2배 디버프: -1
        if (HasDebuff(DebuffFlags.IncreaseRate2x))
            _addAirPerSec += 2f;

        if (HasDebuff(DebuffFlags.Berserk))
            _airDecreaseInterval *= 0.5f;

    }

    #endregion


    public float GetMaxAir()
    {
        return _maxAir;
    }

    public float ShopMaxAirUpgrade()
    {
        _maxAir *= 1.2f;
        return _maxAir;
    }

    public float ShopBaseMinusAirPerSecUpgrade()
    {
        _baseMinusAirPerSec /= 1.2f;
        return _baseMinusAirPerSec;
    }

    public float GetMinusAirPerSec()
    {
        return _minusAirPerSec;
    }

    // StopMinusAirPerSec 버프 처리 함수
    public void StopMinusAirPerSecBuff()
    {
        // _minusAirPerSec 값을 0으로 설정
        _minusAirPerSec = 0;
    }

    // StopMinusAirPerSec 버프 해제 함수
    public void ResetStopMinusAirPerSec()
    {
        // 버프 종료 시 원래 값으로 복원
        _minusAirPerSec = _baseMinusAirPerSec;
    }

    /// <summary>
    /// base 공기 감소 빈도율 감소 함수
    /// </summary>
    public void IncreaseMinusInterval()
    {
        _baseAirDecreaseInterval *= 2f;
    }

    /// <summary>
    /// base 공기 감소 빈도율 증가 함수
    /// </summary>
    public void DecreaseMinusInterval()
    {
        _baseAirDecreaseInterval *= 0.5f;
    }

    /// <summary>
    /// 외부에서 최대 공기량을 증가시키는 함수
    /// </summary>
    public void IncreaseMaxAir()
    {
        _maxAir += 10;
        _curAir += 10;
        _safeAirZone = _maxAir * (2f / 3f);
    }

    /// <summary>
    /// 현재 공기를 설정하는 함수
    /// </summary>
    /// <param name="newValue"></param>
    public void SetAir(float newValue)
    {
        _curAir = newValue;
        onChangeAir?.Invoke(_curAir);

        if (_curAir >= _maxAir || _curAir <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 공기에 변화를 주는 함수 +일 경우 힐 -일 경우 데미지 처리
    /// </summary>
    /// <param name="addValue"></param>
    public void AddAir(float addValue)
    {
        Debug.Log($"AddAir: {addValue}");
        if (addValue < 0)
        {
            HitDamage();        //데미지를 입으면 함수 호출
            if(addValue <= 2)
            {
                OnDamage?.Invoke();
            }   
        }
        SetAir(_curAir + addValue);
    }

    public float GetAir()
    {
        return _curAir; 
    }

    public float GetMaxAirs()
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
        Debug.Log("HitDamage");
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
        if (_minusAirPerSec != 0 && _airDecreaseTimer >= _airDecreaseInterval)
        {
            _airDecreaseTimer = 0f;
            _curAirPerSec = _addAirPerSec - _minusAirPerSec;
            SetAir(Mathf.Max(0, _curAir + _curAirPerSec));
        }
    }
}
