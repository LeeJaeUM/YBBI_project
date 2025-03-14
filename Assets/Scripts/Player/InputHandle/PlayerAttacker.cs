using System.Collections;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour, ISkillType
{
    public PlayerATKStats _aTKStats;

    public NearestEnemyFinder _nearestEnemyFinder;
    public GameObject _bullet;
    public float _curAttackDamage = 1;
    public float _spawnDistance = 1;

    //    [SerializeField]private ISkillType.SkillType _skillType = ISkillType.SkillType.NONE;

    #region Custom Functions 
    private void HandleNormalAttackInput()
    {
        // _skillType = ISkillType.SkillType.Normal;
        Debug.Log("플레이어가 공격 누름");
        Vector3 direction = _nearestEnemyFinder.GetDirectionToNearestEnemy();
        Vector3 spawnPosition = transform.position + direction.normalized * _spawnDistance;

        GameObject creteAttack = Instantiate(_bullet, spawnPosition, Quaternion.identity);
        Bullet bullet = creteAttack.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetArrowVector(direction);

            SkillData skillData = _aTKStats.GetNormalAttackData();
            float damage = _curAttackDamage * skillData._damageMultiplier;
            float radius = skillData._radius;
            float activeTime = skillData._activeTime;
            float speed = skillData._moveSpeed;
            bullet.SetData(damage, radius, activeTime, speed);
        }
    }

    private void HandleAirSkillInput()
    {
        //_skillType = ISkillType.SkillType.Air;
    }
    private void HandleUniqueSkillInput()
    {
        //_skillType = ISkillType.SkillType.Unique;
    }

    #endregion

    // 코루틴으로 쿨타임 관리
    IEnumerator CooldownCoroutine()
    {
        //_isCoolDown = true;
        //_screenButton.enabled = false;
        //float timer = _coolDown;

        //// 버튼 비활성화
        //_baseAtkBtn.interactable = false;
        //_coolDownText.gameObject.SetActive(true);

        //while (timer > 0)
        //{
        //    timer -= Time.deltaTime;

        //    // 쿨타임 UI 업데이트
        //    _coolDownText.text = timer.ToString("F2");

        //    if (_coolDownImg != null)
        //    {
        //        _coolDownImg.fillAmount = 1f - (timer / _coolDown);
        //    }

            yield return null; // 매 프레임마다 반복
        //}

        //// 쿨타임 종료 후
        //_isCoolDown = false;
        //_screenButton.enabled = true;
        //_coolDownText.gameObject.SetActive(false);
        //_baseAtkBtn.interactable = true;

        //if (_coolDownImg != null)
        //{
        //    _coolDownImg.fillAmount = 1f; // 쿨타임 종료 후 이미지 채우기
        //}
    }

    #region Unity Built-in Functions
    void Awake()
    {
        var inputHandler = GetComponent<PlayerInputHandler>(); //입력 핸들러는 이벤트 구독과 해제할 때만 필요하므로, 로컬 변수로 처리

        if (inputHandler != null)   // 이벤트 구독
        {
            inputHandler.OnAttackInput += HandleNormalAttackInput; 
            inputHandler.OnAirSkillInput += HandleAirSkillInput;
            inputHandler.OnUniqueSkillInput += HandleUniqueSkillInput;
        }

        _aTKStats = GetComponent<PlayerATKStats>();
        _nearestEnemyFinder = GetComponentInChildren<NearestEnemyFinder>();
    }
    private void Start()
    {
        _curAttackDamage = _aTKStats.GetAttackDamage();
    }

    void OnDestroy()
    {
        var inputHandler = GetComponent<PlayerInputHandler>();

        if (inputHandler != null)   // 이벤트 해제 (메모리 누수 방지)
        {
            inputHandler.OnUniqueSkillInput -= HandleUniqueSkillInput;
            inputHandler.OnAirSkillInput -= HandleAirSkillInput;
            inputHandler.OnAttackInput -= HandleNormalAttackInput; 
        }
    }
    #endregion
}

