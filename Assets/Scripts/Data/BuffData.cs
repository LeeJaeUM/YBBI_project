using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    None = -1,
    AttackUp,
    MoveSpeedUp,
    StopMinusAirPerSec,
    HealOverTime,
    GamblePlayerMoney,
    Max
}

[Serializable]
public struct BuffData
{
    public BuffType Id;
    public float Duration;
    public float Value;

}

[Serializable]
public struct BuffInfo
{
    public BuffData Data;
    public float Time;
}