using UnityEngine;

public class PlayerATKStats : MonoBehaviour
{
    #region Fields & Properties
    public SkillData _normalAttackSkillData;
    public SkillData _airSkillData;
    public SkillData _uniqueSkillData;

    public float _normalAttackCoolDown = 6.0f;    // 일반 공격 쿨타임
    public float _airSkillCoolDown = 6.0f;        // 공기 스킬 쿨타임 (스킬 1)
    public float _uniqueSkillCoolDown = 6.0f;     // 고유 스킬 쿨타임 (스킬 2)

    public float _attckDamage = 5;


    //public bool _canNormalAttack = true;
    //public bool _canAirSkill = true;
    //public bool _canUniqueSkill = true;

   // public NormalAttack _normalAttack;
   // public AirSkill _airSkill;
   // public UniqueSkill _uniqueSkill;
    #endregion

    #region Custom Functions


    private void StartAttack(BaseSkill baseSkill)
    {

        if (baseSkill == null || !baseSkill.CanAttack()) return;

        baseSkill.Attack();
    }
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
        SetCoolDown(_normalAttackSkillData._coolDown, 
            _airSkillData._coolDown, _uniqueSkillData._coolDown);

       // _normalAttack.SetSkillData(_normalAttackSkillData);
       // _airSkill.SetSkillData(_airSkillData);
       // _uniqueSkill.SetSkillData(_uniqueSkillData);
    }

    public float GetAttackDamage()
    {
        return _attckDamage;
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

    public SkillData GetNormalAttackData()
    {
        return _normalAttackSkillData;
    }
    public SkillData GetAirSkillData()
    {
        return _airSkillData;
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
