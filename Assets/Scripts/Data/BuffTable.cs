using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTable : SingletonMonoBehaviour<BuffTable>
{
    [SerializeField]
    List<BuffData> _buffList = new List<BuffData>();
    Dictionary<BuffType, BuffData> _buffTable = new Dictionary<BuffType, BuffData>();

    public BuffData GetBuff(BuffType buff)
    {
        return _buffTable[buff];
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        for (int i = 0; i < _buffList.Count; i++)
        {
            _buffTable.Add(_buffList[i].Id, _buffList[i]);
        }
    }
}