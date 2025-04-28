using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] public float _speed = 5f;

    private Rigidbody2D _rigid;
    private Vector2 _inputVec;

    [SerializeField] private PlayerAnimator _playerAnimator;

    private void HandleMoveInput(Vector2 input)
    {
        _inputVec = input; // 이동 벡터 업데이트    

        // 이동 애니메이션 관련 처리
        _playerAnimator.UpdateMoveVisual(input);
    }

    public float ShopMoveSpeedUpgrade()
    {
        _speed *= 1.2f;
        return _speed;
    }

    public void UpMoveSpeedBuff(float value)
    {
        _speed *= value;
    }

    public void ResetMoveSpeedBuff(float value)
    {
        _speed /= value;
    }

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        var inputHandler = GetComponent<PlayerInputHandler>(); //입력 핸들러는 이벤트 구독과 해제할 때만 필요하므로, 로컬 변수로 처리

        if (inputHandler != null)
        {
            inputHandler.OnMoveInput += HandleMoveInput; // 이벤트 구독
        }
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    void FixedUpdate()
    {
        Vector2 nextVec = _inputVec * _speed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + nextVec);
    }


    void OnDestroy()
    {
        var inputHandler = GetComponent<PlayerInputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnMoveInput -= HandleMoveInput; // 이벤트 해제 (메모리 누수 방지)
        }
    }
}
