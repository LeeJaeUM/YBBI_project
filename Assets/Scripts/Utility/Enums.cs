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

}
