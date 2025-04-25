using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyATKStats_old : MonoBehaviour
{
    public List<SkillData> _skills;
    public SkillData _curSkill;
    public float _spawnDistance = 1;
    public GameObject _bullet;
    public float _curAttackDamage = 2;
    public float _attackPreDelay = 0.5f;
    public float _attackEndDelay = 1f;

    public Action<bool> OnFinishedAttack;

    public EnemyBulletSpawner _bulletSpawner;

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
        _bulletSpawner.SpawnBulletSpread(
            skillData, _curAttackDamage,
            transform.position, direction);
    }


    IEnumerator CooldownCoroutine(Vector3 direction)
    {
        Debug.Log("공격 시작");
        float coolDown = _curSkill._coolDown;
        yield return new WaitForSeconds(_curSkill._attackPreDelay); //공격전 딜레이만큼 대기
        CreateBullet(_curSkill, direction);
        yield return new WaitForSeconds(_curSkill._attackEndDelay); //공격후 딜레이만큼 대기
        OnFinishedAttack?.Invoke(true);
        yield return new WaitForSeconds(coolDown); //쿨다운 동안 대기
        OnFinishedAttack?.Invoke(false);
    }

    private void Awake()
    {
        _bulletSpawner = GetComponentInChildren<EnemyBulletSpawner>();
        if (_bulletSpawner == null)
        {
            Debug.LogError("EnemyBulletSpawner 컴포넌트를 찾을 수 없습니다.");
        }
    }

}
