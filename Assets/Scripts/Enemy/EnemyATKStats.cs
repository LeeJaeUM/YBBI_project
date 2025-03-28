using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyATKStats : MonoBehaviour
{
    public List<SkillData> _skills;
    public SkillData _curSkill;
    public float _spawnDistance = 1;
    public GameObject _bullet;
    public float _curAttackDamage = 2;
    public float _attackPreDelay = 0.5f;
    public float _attackEndDelay = 2f;

    public Action<bool> OnFinishedAttack;

    public void SetSkillData(int num)
    {
        _curSkill = _skills[num];
    }

    public SkillData GetSKillData(SkillData skillData)
    {
        return _curSkill;
    }

    public void Attack(Vector3 direction)
    {
        StartCoroutine(CooldownCoroutine(direction));
    }
    /// <summary>
    /// 데미지 계산 함수 : 현재는 단순 배율 곱하기
    /// </summary>
    /// <param name="skillDamageMul"></param>
    /// <returns></returns>
    private float GetDamageMultiplier(float skillDamageMul)
    {
        float finalMul = 1;
        finalMul *= skillDamageMul;
        return finalMul;
    }

    private void CreateBullet(SkillData skillData, Vector3 direction)
    {
        Vector3 spawnPosition = transform.position + direction.normalized * _spawnDistance;

        GameObject creteAttack = Instantiate(_bullet, spawnPosition, Quaternion.identity);
        Bullet bullet = creteAttack.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetArrowVector(direction);

            float damage = _curAttackDamage * GetDamageMultiplier(skillData._damageMultiplier);
            float radius = skillData._radius;
            float activeTime = skillData._activeTime;
            float speed = skillData._moveSpeed;
            //TODO : 나중에 스킬 스프라이트 추가 후 Bullet에 전달 추가
            //Sprite sprite = skillData._skillSprite;   
            bullet.SetData(damage, radius, activeTime, speed);
        }
    }


    IEnumerator CooldownCoroutine(Vector3 direction)
    {
        Debug.Log("공격 시작");
        float coolDown = _curSkill._coolDown - _attackPreDelay;
        yield return new WaitForSeconds(_attackPreDelay); //쿨다운 동안 대기
        CreateBullet(_curSkill, direction);
        OnFinishedAttack?.Invoke(true);
        yield return new WaitForSeconds(_attackEndDelay + coolDown); //쿨다운 동안 대기
        OnFinishedAttack?.Invoke(false);
        Debug.Log("공격 시작");
    }
}
