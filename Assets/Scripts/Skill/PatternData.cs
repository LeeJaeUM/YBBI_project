using UnityEngine;


[CreateAssetMenu(fileName = "NewPattern", menuName = "Patterns/Pattern")]
public class PatternData : ScriptableObject
{
    public uint _ID;               // 고유번호 (000~999)
    public string _name;           // 사용자에게 표시될 스킬 이름
    public SkillData[] _skillData; // 스킬 데이터들
    public string _animationName; // 애니메이션 이름  
    public float _coolDown;             // 쿨타임
}
