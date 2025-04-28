using UnityEngine;

public class BulletAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // 해시값 미리 캐싱
    private static readonly int StartTriggerHash = Animator.StringToHash("Start");
    private static readonly int EndTriggerHash = Animator.StringToHash("End");


    //public void UpdateMoveVisual(Vector2 moveDir)
    //{
    //    // 방향 처리
    //    if (moveDir.x < 0)
    //        _spriteRenderer.flipX = true;
    //    else if (moveDir.x > 0)
    //        _spriteRenderer.flipX = false;

    //    // 움직임 애니메이션 처리
    //    bool isMoving = moveDir != Vector2.zero;
    //    _animator.SetBool(IsMovingHash, isMoving);
    //}

    public void PlayStartAnimation()
    {
        _animator.SetTrigger(StartTriggerHash);
    }

    public void PlayEndAnimation()
    {
        _animator.SetTrigger(EndTriggerHash);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
