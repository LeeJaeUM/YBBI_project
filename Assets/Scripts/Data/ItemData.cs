using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Max
}

public enum ItemProperty
{
    Use,
    Make,
    Max
}

[Serializable]
public struct ItemData
{
    public ItemType _id;
    public ItemProperty _property;
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
