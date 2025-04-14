using UnityEngine;


[CreateAssetMenu(fileName = "NewPattern", menuName = "Patterns/Pattern")]
public class PatternData : ScriptableObject
{
    public uint _ID;               // 고유번호 (000~999)
    public string _name;           // 사용자에게 표시될 스킬 이름
    SkillData SkillData; // 스킬 데이터
    public float _bulletCount = 1;   // 투사체 개수 (예: 화살, 불덩이 등)
}
