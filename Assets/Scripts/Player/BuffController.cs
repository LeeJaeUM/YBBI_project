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
            case BuffType.Sample1:
                
                break;
            case BuffType.Sample2:
                
                break;
        }
        while (true)
        {
            curBuff = _activeBuffList[type];

            if (curBuff.Time > curBuff.Data.Duration)
            {
                switch (type)
                {
                    case BuffType.Sample1:
                        
                        break;
                    case BuffType.Sample2:
                        
                        break;
                }
                _activeBuffList.Remove(type);
                yield break;
            }
            curBuff.Time += Time.deltaTime;
            _activeBuffList[type] = curBuff;
            yield return null;
        }
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