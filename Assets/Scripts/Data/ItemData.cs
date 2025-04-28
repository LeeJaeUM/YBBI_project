using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None = -1,
    BlueCrystal,
    DarkBlueCrystal,
    EmeraldCrystal,
    GreenCrystal,
    LightBlueCrystal,
    LilacCrystal,
    OrangeCrystal,
    PinkCrystal,
    PurpleCrystal,
    RedCrystal,
    YellowCrystal,
    AtkUpPotion,
    SpeedUpPotion,
    StopMinusAirPotion,
    HealOverTimePotion,
    GambleMoney
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