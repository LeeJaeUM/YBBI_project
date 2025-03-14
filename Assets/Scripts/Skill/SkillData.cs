using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
public class SkillData : ScriptableObject, ISkillType
{
    public string _skillName;       //스킬 이름
    public ISkillType.SkillType _skillType;  // 스킬 종류
    public Sprite _skillIcon;     // 스킬 아이콘 (스프라이트)
    public float _damageMultiplier = 1.0f;  // 계수
    //public string _attributeType;    // 속성 타입 (예: Fire, Ice 등)
    public float _coolDown;       // 쿨타임
    public float _radius = 1.0f;           //공격 범위
    public float _moveSpeed = 6;
    public float _activeTime = 4;
    public int _level;            // 스킬 레벨
}
