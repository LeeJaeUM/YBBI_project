using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID
{
    ZeZeZe,
    ZeZeOne,
    ZeZeTwo,
    ZeZeThree,
    Max
}

public enum ItemProperty
{
    Use,
    Make,
    UseNow,
    Max
}

[Serializable]
public struct ItemData
{
    public ItemID _id;
    public ItemProperty _property;
    public int _icon;
    public string _name;
    public string _description;

}

[Serializable]
public struct ItemInfo
{
    public ItemData _data;
    public int _count;
}
