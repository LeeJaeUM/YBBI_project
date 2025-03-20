using UnityEngine;
using static Enums;


public class TestEnemy : EnemyAI
{

    protected override void CheckTrigger(Collider2D other)
    {
        base.CheckTrigger(other);
    }
    public override void Initialize()
    {
        base.Initialize();
    }
}
