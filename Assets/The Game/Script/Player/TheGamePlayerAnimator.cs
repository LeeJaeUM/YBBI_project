using UnityEngine;
using Unity.Netcode;
using System.Globalization;
using Unity.Netcode.Components;

public class TheGamePlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkAnimator _networkAnimator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // 애니메이터 파라미터 이름 (문자열 상수)
    private const string IsMovingParam = "IsMoving";
    private const string AttackTriggerParam = "Attack";
    private const string HitTriggerParam = "Hit";
    private const string DieTriggerParam = "Die";
    private const string IsInteractingParam = "IsInteracting";

    /// <summary>
    /// 이동 상태를 동기화하는 NetworkVariable (소유자만 변경 가능)
    /// </summary>
    private readonly NetworkVariable<bool> _isMoving = new NetworkVariable<bool>(
        writePerm: NetworkVariableWritePermission.Owner);

    /// <summary>
    /// 상호작용 중(공격, 피격 등)인지 동기화하는 NetworkVariable (서버만 변경)
    /// </summary>
    private readonly NetworkVariable<bool> _isInteracting = new NetworkVariable<bool>(
        writePerm: NetworkVariableWritePermission.Server);

    /// <summary>
    /// 현재 상호작용 중인지 반환
    /// </summary>
    public bool IsInteracting => _isInteracting.Value;


    /// <summary>
    /// 이동 입력 방향에 따라 이동 애니메이션 및 스프라이트 방향 처리
    /// </summary>
    /// <param name="moveDir">이동 방향 벡터</param>
    public void UpdateMoveVisual(Vector2 moveDir)
    {
        if (!IsOwner) return;

        // 스프라이트 방향 반전
        if (moveDir.x < 0)
            RequestSetFlipX(true);
        else if (moveDir.x > 0)
            RequestSetFlipX(false);

        // 이동 여부를 서버에 알림
        _isMoving.Value = moveDir != Vector2.zero;
    }
    public void RequestSetFlipX(bool isFlip)
    {
        if (_spriteRenderer.flipX == isFlip) return; //스프라이트 반전이 필요없으면 리턴

        if (IsServer)
        {
            SetFlipXInServer(isFlip);
        }
        else
        {
            RequestSetFlipXServerRpc(isFlip); //서버로 스프라이트 반전요청
        }

    }
    public void SetFlipX(bool isFlip)
    {
        _spriteRenderer.flipX = isFlip; //스프라이트 반전
    }
    [ServerRpc]
    public void RequestSetFlipXServerRpc(bool isFlip)
    {
        SetFlipXInServer(isFlip); //서버로 스프라이트 반전요청
    }

    public void SetFlipXInServer(bool isFlip)
    {
        SetFlipX(isFlip); //서버에서 스프라이트 반전
        RequestSetFlipXClientRpc(isFlip); //클라이언트에게 스프라이트 반전 명령
    }
    [ClientRpc]
    public void RequestSetFlipXClientRpc(bool isFlip)
    {
        SetFlipX(isFlip); //스프라이트 반전
    }


    /// <summary>
    /// NetworkVariable의 값 변경 리스너 등록
    /// </summary>
    public override void OnNetworkSpawn()
    {
        _isMoving.OnValueChanged += OnMoveStateChanged;
        _isInteracting.OnValueChanged += OnInteractingChanged;
    }

    /// <summary>
    /// NetworkVariable 리스너 해제
    /// </summary>
    public override void OnNetworkDespawn()
    {
        _isMoving.OnValueChanged -= OnMoveStateChanged;
        _isInteracting.OnValueChanged -= OnInteractingChanged;
    }

    /// <summary>
    /// 이동 상태가 변경되었을 때 애니메이션 파라미터 갱신
    /// </summary>
    private void OnMoveStateChanged(bool previous, bool current)
    {
        _animator.SetBool(IsMovingParam, current);
    }

    /// <summary>
    /// 상호작용 상태가 변경되었을 때 애니메이션 파라미터 갱신
    /// </summary>
    private void OnInteractingChanged(bool previous, bool current)
    {
        _animator.SetBool(IsInteractingParam, current);
    }



    /// <summary>
    /// 공격 애니메이션 재생 및 상호작용 상태 설정
    /// </summary>
    public void PlayAttackAnimation()
    {
        if (!IsOwner) return;

        _networkAnimator.SetTrigger(AttackTriggerParam);
        SetInteractingServerRpc(true);
    }

    /// <summary>
    /// 피격 애니메이션 재생 및 상호작용 상태 설정 (서버 전용)
    /// </summary>
    public void PlayHitAnimation()
    {
        if (!IsServer) return;

        _networkAnimator.SetTrigger(HitTriggerParam);
        _isInteracting.Value = true;
    }

    /// <summary>
    /// 사망 애니메이션 재생 및 상호작용 상태 설정 (서버 전용)
    /// </summary>
    public void PlayDieAnimation()
    {
        if (!IsServer) return;

        _networkAnimator.SetTrigger(DieTriggerParam);
        _isInteracting.Value = true;
    }

    /// <summary>
    /// 상호작용 상태를 서버에 전달
    /// </summary>
    /// <param name="value">상호작용 여부</param>
    [ServerRpc]
    private void SetInteractingServerRpc(bool value)
    {
        _isInteracting.Value = value;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출되어 상호작용 상태를 false로 되돌림
    /// </summary>
    public void OnAnimationEnd()
    {
        if (IsServer)
        {
            _isInteracting.Value = false;
        }
        else if (IsOwner)
        {
            SetInteractingServerRpc(false);
        }
    }
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
