using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyATKStats : MonoBehaviour
{
    //    public List<PatternData> _allPhasePatterns; //페이즈 당 패턴들
    [SerializeField] private List<PhasePatternData> phasePatterns;
    [SerializeField] private int _curPhaseNum = 0; //현재 페이즈 번호
    [SerializeField] private List<PatternData> _patterns;
    [SerializeField] private PatternData _curPattern;     //현재 사용중인 패턴
    [SerializeField] private SkillData _curSkill;         //패턴 속 현재 사용중인 스킬
    public float _spawnDistance = 1;
    public GameObject _bullet;
    public float _curAttackDamage = 2;
    public float _attackPreDelay = 0.5f;
    public float _attackEndDelay = 1f;

    public Action<bool> OnFinishedAttack;
    public Action<float, float, float> OnMoveSkill;

    public EnemyBulletSpawner _bulletSpawner;
    private EnemyAnimator _enemyAnimator;

    public bool useRPC = false; //RPC 사용 여부
    public RPC_BulletSpawner _rpcBulletSpawner; //RPC 사용시 사용

    public void SetPatternNum(int num)
    {
        _curPattern = _patterns[num];
    }

    public void SetPhaseNum(bool isReturnNormal)    //현재 페이즈의 패턴으로 모든 패턴 교체
    {
        if (isReturnNormal)
        {
            _curPhaseNum--;
            _curPhaseNum = Mathf.Max(0, _curPhaseNum); //0 이하 방지
        }
        else
            _curPhaseNum++;

        _patterns = phasePatterns[_curPhaseNum].patterns;
        _enemyAnimator.PlayPhaseChange();
    }

    public int GetPhaseNum()
    {
        return _curPhaseNum;
    }

    //public SkillData GetSKillData(SkillData skillData, int num)
    //{
    //    Debug.Log($"스킬 데이터 현재 미사용 함수 {skillData._name} {num}");
    //    return null;// _curPattern;
    //}

    public void Attack(Vector3 direction)
    {
        StartCoroutine(CooldownCoroutine(direction));
    }

    private void CreateBullet(SkillData skillData, Vector3 direction)
    {
        if(!useRPC) //RPC 사용하지 않을 경우
        {
            _bulletSpawner.SpawnBulletSpread(false,
                skillData, _curAttackDamage,
                transform.position, direction);
        }
        else //RPC 사용시
        {
            _rpcBulletSpawner.RequestSpawnBulletSpread(false, 
                skillData, _curAttackDamage,
                transform.position, direction);
        }
    }


    IEnumerator CooldownCoroutine(Vector3 direction)
    {
        _enemyAnimator.PlayPatternAniamation(_curPattern._animationName); //패턴 애니메이션 실행

        for (int i=0; i< _curPattern._skillData.Length; i++) //패턴에 있는 모든 스킬 연속으로 실행 후 종료
        {
            _curSkill = _curPattern._skillData[i];
            Debug.Log($"스킬 데이터 {_curSkill._name} {i}  공격 시작");



            yield return new WaitForSeconds(_curSkill._attackPreDelay); //공격전 딜레이만큼 대기
            if(_curSkill._isMoveSkill) //이동 스킬일 경우
            {
                OnMoveSkill?.Invoke(_curSkill._moveDistance, _curSkill._moveSpeed, _curSkill._activeTime); //이동 스킬 시작
                yield return new WaitForSeconds(_curSkill._activeTime); //이동 스킬 지속시간 만큼 대기
            }
            else
            {
                CreateBullet(_curSkill, direction);
            }
            yield return new WaitForSeconds(_curSkill._attackEndDelay); //공격후 딜레이만큼 대기
        }

        float coolDown = _curPattern._coolDown;
        OnFinishedAttack?.Invoke(true);
        yield return new WaitForSeconds(coolDown); //쿨다운 동안 대기
        OnFinishedAttack?.Invoke(false);
    }

    private void Awake()
    {
        _patterns = phasePatterns[0].patterns; //초기 패턴 설정
        _bulletSpawner = GetComponentInChildren<EnemyBulletSpawner>();
        _enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        _rpcBulletSpawner = GetComponentInChildren<RPC_BulletSpawner>();
        if (_bulletSpawner == null)
        {
            Debug.LogError("EnemyBulletSpawner 컴포넌트를 찾을 수 없습니다.");
        }
    }
}
