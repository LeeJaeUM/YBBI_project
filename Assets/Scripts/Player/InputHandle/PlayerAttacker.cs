using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISkillType;

public class PlayerAttacker : MonoBehaviour, ISkillType
{
    public PlayerATKStats _aTKStats;

    public NearestEnemyFinder _nearestEnemyFinder;
    public GameObject _bullet;
    public float _curAttackDamage = 1;
    public float _spawnDistance = 1;
    [SerializeField]
    private Dictionary<SkillType, float> _lastUsedTime = new Dictionary<SkillType, float>();

    #region Custom Functions 

    /// <summary>
    /// 각 스킬타입마다 동일한 기능을 모든 함수
    /// </summary>
    /// <param name="skillType">사용할 스킬타입 : ISkillType.SkillType </param>
    private void CreateAttack(ISkillType.SkillType skillType)
    {
        SkillData skillData;        
        switch (skillType)
        {
            case ISkillType.SkillType.Normal:
                skillData = _aTKStats.GetNormalAttackData();
                break;
            case ISkillType.SkillType.Pressure:
                skillData = _aTKStats.GetPressureSkillData();
                break;
            case ISkillType.SkillType.Unique:
                skillData = _aTKStats.GetUniqueSkillData();
                break;
            default:
                Debug.LogWarning("현재 사용할 스킬이 NULL입니다!!");
                return;
        }
        if (!CanUseSkill(skillData))
        {
            //Debug.Log($"스킬 {skillData._name}은 아직 사용 불가!");
            return;
        }
        _lastUsedTime[skillData._skillType] = Time.time;  // 해당 타입의 스킬 쿨타임 업데이트
        Debug.Log($"스킬 {skillData._ID}은 아직 사용 불가!");

        Vector3 direction = _nearestEnemyFinder.GetDirectionToNearestEnemy();
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

    /// <summary>
    /// 현재 스킬 데이터의 쿨타임과 비교해서 사용가능하면 true
    /// </summary>
    /// <param name="skillData"></param>
    /// <returns>사용가능하면 true</returns>
    public bool CanUseSkill(SkillData skillData)
    {
        float lastUsed = _lastUsedTime[skillData._skillType];
        return Time.time >= lastUsed + skillData._coolDown;
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


    private void HandleNormalAttackInput()
    {
        ISkillType.SkillType skillType = ISkillType.SkillType.Normal;
        CreateAttack(skillType);
    }

    private void HandlePressureSkillInput(bool isPressed)
    {
        Debug.Log($"눌림 {isPressed}");
        ISkillType.SkillType skillType = ISkillType.SkillType.Pressure;
        CreateAttack(skillType);
    }
    private void HandleUniqueSkillInput()
    {
        ISkillType.SkillType skillType = ISkillType.SkillType.Unique;
        CreateAttack(skillType);
    }

    #endregion

    // 코루틴으로 쿨타임 관리
    IEnumerator CooldownCoroutine()
    {
            yield return null; // 매 프레임마다 반복
    }

    #region Unity Built-in Functions
    void Awake()
    {
        var inputHandler = GetComponent<PlayerInputHandler>(); //입력 핸들러는 이벤트 구독과 해제할 때만 필요하므로, 로컬 변수로 처리

        if (inputHandler != null)   // 이벤트 구독
        {
            inputHandler.OnAttackInput += HandleNormalAttackInput; 
            inputHandler.OnPressureSkillInput += HandlePressureSkillInput;
            inputHandler.OnUniqueSkillInput += HandleUniqueSkillInput;
        }

        _aTKStats = GetComponent<PlayerATKStats>();                     //GetComponent
        _nearestEnemyFinder = GetComponentInChildren<NearestEnemyFinder>();
    }
    private void Start()
    {
        _curAttackDamage = _aTKStats.GetAttackDamage();

        // 모든 스킬 타입의 마지막 사용 시간을 0으로 초기화
        foreach (SkillType type in System.Enum.GetValues(typeof(SkillType)))
        {
            _lastUsedTime[type] = 0f;
        }
    }

    void OnDestroy()
    {
        var inputHandler = GetComponent<PlayerInputHandler>();

        if (inputHandler != null)   // 이벤트 해제 (메모리 누수 방지)
        {
            inputHandler.OnUniqueSkillInput -= HandleUniqueSkillInput;
            inputHandler.OnPressureSkillInput -= HandlePressureSkillInput;
            inputHandler.OnAttackInput -= HandleNormalAttackInput; 
        }
    }
    #endregion
}

