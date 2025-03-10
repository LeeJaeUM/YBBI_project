using UnityEngine;

public interface ISkillType
{
    public enum SkillType
    {
        NONE,

        Normal, // 기본 공격
        Air,    // 공중 스킬
        Unique  // 고유 스킬
    }

}
