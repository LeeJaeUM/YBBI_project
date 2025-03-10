using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public PlayerATKStats _aTKStats;

    #region Custom Functions 
    private void HandleNormalAttackInput()
    {
        Debug.Log("플레이어가 기본공격 누름");
    }

    private void HandleAirSkillInput()
    {

    }
    private void HandleUniqueSkillInput()
    {

    }
    #endregion

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

