using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // 해시값 미리 캐싱
    private static readonly int IsMovingHash            = Animator.StringToHash("IsMoving");
    private static readonly int AttackTriggerHash       = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash          = Animator.StringToHash("Hit");
    private static readonly int DieTriggerHash          = Animator.StringToHash("Die");
    private static readonly int IsInteractingHash       = Animator.StringToHash("IsInteracting");

    public bool IsInteracting => _animator.GetBool(IsInteractingHash);

    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void UpdateMoveVisual(Vector2 moveDir)
    {
        // 방향 처리
        if (moveDir.x < 0)
            _spriteRenderer.flipX = true;
        else if (moveDir.x > 0)
            _spriteRenderer.flipX = false;

        // 움직임 애니메이션 처리
        bool isMoving = moveDir != Vector2.zero;
        _animator.SetBool(IsMovingHash, isMoving);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger(AttackTriggerHash);
        _animator.SetBool(IsInteractingHash, true);
    }

    public void PlayHitAnimation()
    {
        _animator.SetTrigger(HitTriggerHash);
        _animator.SetBool(IsInteractingHash, true);
    }

    public void PlayDieAnimation()
    {
        _animator.SetTrigger(DieTriggerHash);
        _animator.SetBool(IsInteractingHash, true);
    }

    // 애니메이션 이벤트에서 호출
    public void OnAnimationEnd()
    {
        _animator.SetBool(IsInteractingHash, false);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
