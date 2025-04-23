using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BuffController : MonoBehaviour
{
    [SerializeField]
    public PlayerItemController _player;

    Dictionary<BuffType, BuffInfo> _activeBuffList = new Dictionary<BuffType, BuffInfo>();

    IEnumerator CoBuffProcess(BuffType type)
    {
        BuffInfo curBuff;

        switch (type)
        {
            case BuffType.AttackUp:
                // _player.AttackPower += curBuff.Data.Value;
                break;
            case BuffType.MoveSpeedUp:
                // _player.MoveSpeed *= curBuff.Data.Value;
                break;
            case BuffType.Invincibility:
                // _player.IsInvincible = true;
                break;
        }

        while (true)
        {
            curBuff = _activeBuffList[type];

            if (curBuff.Time > curBuff.Data.Duration)
            {
                break;
            }

            curBuff.Time += Time.deltaTime;
            _activeBuffList[type] = curBuff;
            yield return null;
        }

        switch (type)
        {
            case BuffType.AttackUp:
                // _player.AttackPower -= curBuff.Data.Value;
                break;
            case BuffType.MoveSpeedUp:
                // _player.MoveSpeed /= curBuff.Data.Value;
                break;
            case BuffType.Invincibility:
                // _player.IsInvincible = false;
                break;
        }

        _activeBuffList.Remove(type);
    }

    public void SetBuff(BuffType buff)
    {
        BuffInfo curBuff;
        if (_activeBuffList.TryGetValue(buff, out curBuff))
        {
            curBuff.Time = 0;
            _activeBuffList[buff] = curBuff;
        }
        else
        {
            curBuff = new BuffInfo() { Data = BuffTable.Instance.GetBuff(buff), Time = 0 };
            _activeBuffList.Add(buff, curBuff);
            StartCoroutine(CoBuffProcess(buff));
        }

    }
}