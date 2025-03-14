using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    protected SkillData _skillData;

    public void SetSkillData(SkillData data)
    {
        _skillData = data;
    }

    public abstract bool CanAttack();
    public virtual void Attack()
    {

    }
}
