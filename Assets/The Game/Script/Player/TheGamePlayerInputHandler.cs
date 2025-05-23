using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 모든 인풋은 여기서 받고 액션으로 다른 스크립트로 입력값을 보냄
/// 입력을 핸들러에서 관리하여 모든 입력을 볼 수 있음
/// </summary>
public class TheGamePlayerInputHandler : NetworkBehaviour 
{
    public event Action<Vector2> OnMoveInput; // 이동 입력 이벤트
    public event Action<bool> OnAttackInput; // 공격 입력 이벤트
    public event Action OnPressureSkillInput; // 압력 스킬 입력 이벤트
    public event Action OnUniqueSkillInput; // 유니크 스킬 입력 이벤트

    private void Start()
    {
        //타 플레이어의 입력은 끔
        if(!IsLocalPlayer)
        {
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        if (!IsOwner) return; 
        OnMoveInput?.Invoke(value.ReadValue<Vector2>()); // 이동 입력이 들어올 때만 이벤트 실행
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (!IsOwner) return;
        if (value.started)
            OnAttackInput?.Invoke(true);
        else if(value.canceled)
            OnAttackInput?.Invoke(false);
    }

    public void OnPressureSkill(InputAction.CallbackContext value)
    {
        if (!IsOwner) return;
        OnPressureSkillInput?.Invoke();
    }

    public void OnUniqueSkill(InputAction.CallbackContext value)
    {
        if (!IsOwner) return;
        OnUniqueSkillInput?.Invoke();
    }

    private void TestInputCheck(InputAction.CallbackContext value, string name)
    {
        Debug.Log($"{name} : {value.started}, {value.performed},  {value.canceled} ");
    }
}
