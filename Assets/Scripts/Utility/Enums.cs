public static class Enums
{
    public enum SkillType
    {
        NONE,

        Normal,     // 기본 공격
        Pressure,   // 전투 스킬 (압력)
        Unique      // 고유 스킬
    }

    public enum EnemyStateType 
    {
        NONE,

        Idle, 
        Patrol, 
        Chase, 
        Attack
    }

    public enum BulletType
    {
        NONE,

        Normal,     // 일반 총알
        Homing,     // 유도 총알
        Bomb,       // 폭탄
        Laser,      // 레이저
        Mine       // 지뢰
    }
    public enum ATKPatern
    {
        A,
        B,
        C,
        D,
        E,
        F
    }

    [System.Flags]
    public enum DebuffFlags
    {
        None = 0,

        DecreaseRate2x = 1 << 0,   // 00000001
        IncreaseRate2x = 1 << 1,   // 00000010

        Poison = 1 << 2,           // 00000100
        Berserk = 1 << 3,          // 00001000
        Shock = 1 << 4             // 00010000
    }
}
