using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None = -1,
    Arrow,
    BeeHouse,
    Bomb,
    Key,
    Mashroom,
    MoneyAdjust
}

public enum ItemProperty
{
    Use,
    Make,
    Sell
}

[Serializable]
public struct ItemData
{
    public ItemType _id;
    public ItemProperty _property;
    public BuffType _buff;
    public int _icon;
    public float _value;
    public int _price;
}

[Serializable]
public struct ItemInfo
{
    public ItemData _data;
    public int _count;
}