using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataTable : MonoBehaviour
{
    [SerializeField]
    ItemData[] itemDatas;
    public Dictionary<ItemID, ItemData> _itemTable = new Dictionary<ItemID, ItemData>();
    public ItemData this[ItemID Data]
    {
        get
        {
            return _itemTable[Data];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            ItemData data = itemDatas[i];
            _itemTable.Add(itemDatas[i]._id, data);
        }
    }
}
