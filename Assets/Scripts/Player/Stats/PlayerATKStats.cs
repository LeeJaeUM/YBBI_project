using UnityEngine;

public class PlayerATKStats : MonoBehaviour
{
    #region Fields & Properties
    public SkillData _normalAttackSkill;
    public SkillData _airSkill;
    public SkillData _uniqueSkill;

    public float _normalAttackCoolDown = 6.0f;    // 일반 공격 쿨타임
    public float _airSkillCoolDown = 6.0f;        // 공기 스킬 쿨타임 (스킬 1)
    public float _uniqueSkillCoolDown = 6.0f;     // 고유 스킬 쿨타임 (스킬 2)
    #endregion

    #region Custom Functions
    private void SetCoolDown(float normalCool, float airCool, float uniqueCool)
    {
        _normalAttackCoolDown = normalCool;
        _airSkillCoolDown = airCool;
        _uniqueSkillCoolDown = uniqueCool;
    }

    /// <summary>
    /// 시작 시 SkillData의 내용 불러오기
    /// </summary>
    private void InitSkillData()
    {
        SetCoolDown(_normalAttackSkill._coolDown, 
            _airSkill._coolDown, _uniqueSkill._coolDown);
    }

    public float GetNormalCoolDown()
    {
        return _normalAttackCoolDown;
    }

    public float GetAirSkillCoolDown()
    {
        return _airSkillCoolDown;
    }

    public float GetUniqueSkillCoolDown()
    {
        return _uniqueSkillCoolDown;
    }
    #endregion

    private void Start()
    {
        InitSkillData();
    }

}
