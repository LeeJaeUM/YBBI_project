using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{

    private void HandleAttackInput()
    {
        Debug.Log("플레이어가 기본공격 누름");
    }
    void Awake()
    {
        var inputHandler = GetComponent<PlayerInputHandler>(); //입력 핸들러는 이벤트 구독과 해제할 때만 필요하므로, 로컬 변수로 처리

        if (inputHandler != null)
        {
            inputHandler.OnAttackInput += HandleAttackInput; // 이벤트 구독
        }
    }

    void OnDestroy()
    {
        var inputHandler = GetComponent<PlayerInputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnAttackInput -= HandleAttackInput; // 이벤트 해제 (메모리 누수 방지)
        }
    }

 
}
