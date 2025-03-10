using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 모든 인풋은 여기서 받고 액션으로 다른 스크립트로 입력값을 보냄
/// 입력을 핸들러에서 관리하여 모든 입력을 볼 수 있음
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput; // 이동 입력 이벤트
    public event Action OnAttackInput; // 공격 입력 이벤트

    void OnMove(InputValue value)
    {
        OnMoveInput?.Invoke(value.Get<Vector2>()); // 이동 입력이 들어올 때만 이벤트 실행
    }

    void OnAttack(InputValue value)
    {
        OnAttackInput?.Invoke();
    }
}
