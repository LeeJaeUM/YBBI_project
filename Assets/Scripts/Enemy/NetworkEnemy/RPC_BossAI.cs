using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class RPC_BossAI : RPC_EnemyAI
{
    public override void CreateState()
    {
        _states = new Dictionary<EnemyStateType, IEnemyState>
        {
            { EnemyStateType.Idle, new BossIdleState() },
            { EnemyStateType.Attack, new BossAttackState() }
        };
    }
}
