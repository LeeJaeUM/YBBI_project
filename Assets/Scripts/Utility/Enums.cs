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

}
