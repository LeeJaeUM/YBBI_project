using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
public class SkillData : ScriptableObject
{
    public uint _ID;               // 고유번호 (000~999)
    public string _name;           // 사용자에게 표시될 스킬 이름
    public Sprite _icon;           // 스킬 버튼 아이콘 (스프라이트)
    public Sprite _sprite;         // 스킬 이미지/이펙트 (스프라이트)
    public Enums.SkillType _skillType;  // 스킬 종류
    public float _damageMultiplier = 1.0f;  // 계수
    //public string _attributeType;     // 속성 타입 (예: Fire, Ice 등)
    public float _coolDown;             // 쿨타임
    public float _radius = 1.0f;        //공격 범위
    public float _moveSpeed = 3;        //투사체 속도
    public float _activeTime = 3;       //투사체 유지 시간
    public float _attackPreDelay = 0;   //공격 전 딜레이
    public float _attackEndDelay = 0;   // 공격 후 딜레이
    public int _level;                  // 스킬 레벨
}
