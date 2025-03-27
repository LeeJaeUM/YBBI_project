using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemDataTable : MonoBehaviour
{
    [SerializeField]
    ItemData[] itemDatas;
    public Dictionary<ItemType, ItemData> _itemTable = new Dictionary<ItemType, ItemData>();
    public ItemData this[ItemType Data]
    {
        get
        {
            return _itemTable[Data];
        }
    }

    //public ItemData GetData(ItemType type)
    //{
    //    return _itemTable[type];
    //}

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
