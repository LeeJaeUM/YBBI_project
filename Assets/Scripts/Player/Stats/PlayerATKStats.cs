using UnityEngine;

public class PlayerATKStats : MonoBehaviour
{
    #region Fields & Properties
    public SkillData _normalAttackSkillData;
    public SkillData _pressureSkillData;
    public SkillData _uniqueSkillData;

    public float _normalAttackCoolDown = 6.0f;    // 일반 공격 쿨타임
    public float _pressureSkillCoolDown = 6.0f;        // 공기 스킬 쿨타임 (스킬 1)
    public float _uniqueSkillCoolDown = 6.0f;     // 고유 스킬 쿨타임 (스킬 2)

    public float _attckDamage = 5;
    #endregion

    #region Custom Functions
    private void SetCoolDown(float normalCool, float pressureCool, float uniqueCool)
    {
        _normalAttackCoolDown = normalCool;
        _pressureSkillCoolDown = pressureCool;
        _uniqueSkillCoolDown = uniqueCool;
    }

    /// <summary>
    /// 시작 시 SkillData의 내용 불러오기
    /// </summary>
    private void InitSkillData()
    {
        SetCoolDown(_normalAttackSkillData._coolDown, 
            _pressureSkillData._coolDown, _uniqueSkillData._coolDown);
    }

    public float GetAttackDamage()
    {
        return _attckDamage;
    }

    public float GetNormalCoolDown()
    {
        return _normalAttackCoolDown;
    }

    public float GetPressureSkillCoolDown()
    {
        return _pressureSkillCoolDown;
    }

    public float GetUniqueSkillCoolDown()
    {
        return _uniqueSkillCoolDown;
    }

    public SkillData GetNormalAttackData()
    {
        return _normalAttackSkillData;
    }
    public SkillData GetPressureSkillData()
    {
        return _pressureSkillData;
    }
    public SkillData GetUniqueSkillData()
    {
        return _uniqueSkillData;
    }
    #endregion

    private void Awake()
    {
        InitSkillData();
    }

}
