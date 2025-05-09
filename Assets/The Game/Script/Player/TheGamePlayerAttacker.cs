using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class TheGamePlayerAttacker : NetworkBehaviour
{
    public PlayerATKStats _aTKStats;

    public NearestEnemyFinder _nearestEnemyFinder;
    public GameObject _bullet;
    public float _curAttackDamage = 1;
    public float _spawnDistance = 1;
    [SerializeField]
    private Dictionary<Enums.SkillType, float> _lastUsedTime = new Dictionary<Enums.SkillType, float>();

    public bool _isPressed = false;
    private Vector2 _inputVec = Vector2.zero;       //이동방향 : 적이 없을 시 공격할 때 사용
    private TheGamePlayerAnimator _playerAnimator;

    public bool useRPC = true; //RPC 사용 여부
    public RPC_BulletSpawner _rpcBulletSpawner; //RPC 사용시 사용

    #region Custom Functions 

    /// <summary>
    /// 각 스킬타입마다 동일하게 공격 시작 함수
    /// </summary>
    /// <param name="Enums.SkillType">사용할 스킬타입 : IEnums.SkillType.Enums.SkillType </param>
    private void StartAttack(Enums.SkillType skillType)
    {
        SkillData skillData = SelectSkillData(skillType);
        if (skillData == null)
            return;

        if (!CanUseSkill(skillData))
        {
            //Debug.Log($"스킬 {skillData._name}은 아직 사용 불가!");
            return;
        }
        Vector3 direction = _nearestEnemyFinder.GetDirectionToNearestEnemy();
        CreateBullet(skillData, direction);
    }

    /// <summary>
    /// skillData만 받는 공격 시작 함수
    /// </summary>
    /// <param name="skillData">이번 스킬의 skillData</param>
    private void StartAttackInCor(SkillData skillData)
    {
        if (!CanUseSkill(skillData))
        {
            //Debug.Log($"스킬 {skillData._name}은 아직 사용 불가!");
            return;
        }

        Vector3 direction = _nearestEnemyFinder.GetDirectionToNearestEnemy();
        CreateBullet(skillData, direction);
    }

    /// <summary>
    /// enum타입에 맞게 skillData를 ATKStats에서 불러옴
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    private SkillData SelectSkillData(Enums.SkillType skillType)
    {
        SkillData skillData;
        switch (skillType)
        {
            case Enums.SkillType.Normal:
                skillData = _aTKStats.GetNormalAttackData();
                break;
            case Enums.SkillType.Pressure:
                skillData = _aTKStats.GetPressureSkillData();
                break;
            case Enums.SkillType.Unique:
                skillData = _aTKStats.GetUniqueSkillData();
                break;
            default:
                Debug.LogWarning("현재 사용할 스킬이 NULL입니다!!");
                return null;
        }
        return skillData;
    }

    /// <summary>
    /// 현재 스킬 데이터의 쿨타임과 비교해서 사용가능하면 true
    /// </summary>
    /// <param name="skillData"></param>
    /// <returns>사용가능하면 true</returns>
    public bool CanUseSkill(SkillData skillData)
    {
        bool value = false;
        float lastUsed = _lastUsedTime[skillData._skillType];

        value = Time.time >= lastUsed + skillData._coolDown;
        if (value)
        {
            _lastUsedTime[skillData._skillType] = Time.time;  // 해당 타입의 스킬 쿨타임 업데이트
        }

        return value;
    }

    //private void CreateBullet(SkillData skillData)
    //{
    //    Vector3 direction = _nearestEnemyFinder.GetDirectionToNearestEnemy();
    //    Debug.Log($"{direction}");
    //    if (direction == Vector3.zero)
    //    {
    //        direction = _inputVec;
    //        Debug.Log($"{direction}");
    //    }
    //    Vector3 spawnPosition = transform.position + direction.normalized * _spawnDistance;

    //    GameObject creteAttack = Instantiate(_bullet, spawnPosition, Quaternion.identity);
    //    Bullet bullet = creteAttack.GetComponent<Bullet>();
    //    if (bullet != null)
    //    {
    //        bullet.SetArrowVector(direction);

    //        float damage = _curAttackDamage * GetDamageMultiplier(skillData._damageMultiplier);
    //        float radius = skillData._radius;
    //        float activeTime = skillData._activeTime;
    //        float speed = skillData._moveSpeed;
    //        //TODO : 나중에 스킬 스프라이트 추가 후 Bullet에 전달 추가
    //        //Sprite sprite = skillData._skillSprite;   
    //        bullet.SetData(true, damage, radius, skillData._width, skillData._length, activeTime, speed);

    //        _playerAnimator.PlayAttackAnimation();  //공격 애니메이션 실행
    //    }
    //}
    private void CreateBullet(SkillData skillData, Vector3 direction)
    {
        if (useRPC) //RPC 사용
        {
            _rpcBulletSpawner.RequestSpawnBulletSpread(true,
                skillData, _curAttackDamage,
                transform.position, direction);
        }

        _playerAnimator.PlayAttackAnimation();  //공격 애니메이션 실행
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


    private void HandleNormalAttackInput(bool isPressed)
    {
        Debug.Log($"눌림 {isPressed}");
        _isPressed = isPressed;
        Enums.SkillType skillType = Enums.SkillType.Normal;

        //버튼 릴리즈 시 실행중인 코루틴 중지
        if (!isPressed)
            StopAllCoroutines();

        // 홀드용 코루틴 시작 
        StartCoroutine(CooldownCoroutine(skillType));
    }

    private void HandlePressureSkillInput()
    {
        Enums.SkillType skillType = Enums.SkillType.Pressure;
        StartAttack(skillType);
    }
    private void HandleUniqueSkillInput()
    {
        Enums.SkillType skillType = Enums.SkillType.Unique;
        StartAttack(skillType);
    }


    private void HandleMoveInput(Vector2 vector)
    {
        if(vector != Vector2.zero)
            _inputVec = vector;
    }

    #endregion

    /// <summary>
    /// 꾹 누를때 코루틴 실행하여 쿨타임마다 스킬 사용
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    IEnumerator CooldownCoroutine(Enums.SkillType skillType)
    {
        SkillData skillData = SelectSkillData(skillType);
        float coolDown = skillData._coolDown + 0.01f;
        while (_isPressed)
        {
            StartAttackInCor(skillData);
            yield return new WaitForSeconds(coolDown); // 쿨타임 마다 반복
        }
        yield return null;
    }

    #region Unity Built-in Functions
    void Awake()
    {
        var inputHandler = GetComponent<TheGamePlayerInputHandler>(); //입력 핸들러는 이벤트 구독과 해제할 때만 필요하므로, 로컬 변수로 처리

        if (inputHandler != null)   // 이벤트 구독
        {
            inputHandler.OnAttackInput += HandleNormalAttackInput;
            inputHandler.OnPressureSkillInput += HandlePressureSkillInput;
            inputHandler.OnUniqueSkillInput += HandleUniqueSkillInput;
            inputHandler.OnMoveInput += HandleMoveInput;
        }

        _aTKStats = GetComponent<PlayerATKStats>();                     //GetComponent
        _nearestEnemyFinder = GetComponentInChildren<NearestEnemyFinder>();
        _playerAnimator = GetComponent<TheGamePlayerAnimator>();
        _rpcBulletSpawner = GetComponent<RPC_BulletSpawner>();
    }


    private void Start()
    {
        _curAttackDamage = _aTKStats.GetAttackDamage();

        // 모든 스킬 타입의 마지막 사용 시간을 0으로 초기화
        foreach (Enums.SkillType type in System.Enum.GetValues(typeof(Enums.SkillType)))
        {
            _lastUsedTime[type] = 0f;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        var inputHandler = GetComponent<TheGamePlayerInputHandler>();

        if (inputHandler != null)   // 이벤트 해제 (메모리 누수 방지)
        {
            inputHandler.OnMoveInput -= HandleMoveInput;
            inputHandler.OnUniqueSkillInput -= HandleUniqueSkillInput;
            inputHandler.OnPressureSkillInput -= HandlePressureSkillInput;
            inputHandler.OnAttackInput -= HandleNormalAttackInput;
        }
    }
    #endregion
}

