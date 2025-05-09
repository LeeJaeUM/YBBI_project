using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 플레이어에게 버프 효과를 적용하고 해제하는 컨트롤러 클래스
/// </summary>
public class BuffController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    public PlayerItemController _player;

    #endregion

    #region Private Variables

    // 플레이어의 스탯 및 상태 관련 컴포넌트
    PlayerATKStats _playerATKStats;
    PlayerMover _playerMover;
    UnitHealth _unitHealth;
    Shop _shop;

    #endregion

    #region Buff Management

    // 현재 활성화된 버프 목록
    Dictionary<BuffType, BuffInfo> _activeBuffList = new Dictionary<BuffType, BuffInfo>();

    #endregion

    #region Buff Processing Coroutine

    /// <summary>
    /// 특정 버프 타입의 효과를 일정 시간 동안 적용하는 코루틴
    /// </summary>
    IEnumerator CoBuffProcess(BuffType type)
    {
        // 현재 버프가 등록되어 있지 않으면 종료
        if (!_activeBuffList.TryGetValue(type, out BuffInfo curBuff))
        {
            yield break;
        }

        float healInterval = 1f;
        float healTimer = 0f;

        // 버프 종류에 따라 효과 적용
        switch (type)
        {
            case BuffType.AttackUp:
                _playerATKStats.UpAttackBuff(curBuff.Data.Value);
                break;

            case BuffType.MoveSpeedUp:
                _playerMover.UpMoveSpeedBuff(curBuff.Data.Value);
                break;

            case BuffType.StopMinusAirPerSec:
                if (_unitHealth != null)
                {
                    // StopMinusAirPerSec 버프를 적용
                    _unitHealth.StopMinusAirPerSecBuff();
                }
                break;

            case BuffType.HealOverTime:
                // HealOverTime은 별도 로직이 필요할 수 있음
                break;

            case BuffType.GamblePlayerMoney:
                _shop.GamblePlayerMoney();
                break;
        }

        // 버프 지속 시간 동안 대기
        while (true)
        {
            if (!_activeBuffList.TryGetValue(type, out curBuff))
                break;

            if (curBuff.Time > curBuff.Data.Duration)
                break;

            // HealOverTime일 경우 일정 주기로 HealAirBuff() 호출
            if (type == BuffType.HealOverTime)
            {
                healTimer += Time.deltaTime;
                if (healTimer >= healInterval)
                {
                    _unitHealth.HealAirBuff();
                    healTimer = 0f;
                }
            }

            curBuff.Time += Time.deltaTime;
            _activeBuffList[type] = curBuff;
            yield return null;
        }

        // 버프 종료 후 원상 복구 처리
        switch (type)
        {
            case BuffType.AttackUp:
                _playerATKStats.ResetAttackBuff(curBuff.Data.Value);
                break;

            case BuffType.MoveSpeedUp:
                _playerMover.ResetMoveSpeedBuff(curBuff.Data.Value);
                break;

            case BuffType.StopMinusAirPerSec:  
                _unitHealth.ResetStopMinusAirPerSec();  
                break;

            case BuffType.HealOverTime:
                // HealOverTime 종료 처리
                break;

            case BuffType.GamblePlayerMoney:
                // 종료 시 실행할 내용은 없음
                break;
        }
        // 버프 목록에서 제거
        _activeBuffList.Remove(type);
    }

    #endregion

    #region Buff Setting Method

    /// <summary>
    /// 외부에서 버프를 적용 요청할 때 호출하는 함수
    /// </summary>
    public void SetBuff(BuffType buff)
    {
        BuffInfo curBuff;
        if (_activeBuffList.TryGetValue(buff, out curBuff))
        {
            // 이미 적용 중이면 시간 초기화 (버프 갱신)
            curBuff.Time = 0;
            _activeBuffList[buff] = curBuff;
        }
        else
        {
            // 새 버프 적용
            curBuff = new BuffInfo() { Data = BuffTable.Instance.GetBuff(buff), Time = 0 };
            _activeBuffList.Add(buff, curBuff);
            StartCoroutine(CoBuffProcess(buff));
        }
    }

    #endregion
}