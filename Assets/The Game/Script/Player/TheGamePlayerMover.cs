using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class TheGamePlayerMover : NetworkBehaviour
{

    [SerializeField] private float _speed = 5f;

    private Rigidbody2D _rigid;
    private Vector2 _inputVec;
    private PlayerPosRPC _playerPosRpc;
    private TheGamePlayerAnimator _playerAnimator;

    private void HandleMoveInput(Vector2 input)
    {
        if (!IsLocalPlayer) return;

        _inputVec = input;
        // 이동 애니메이션 관련 처리
        _playerAnimator.UpdateMoveVisual(input);

        _playerPosRpc.MoveRequest(_inputVec);
        if (input == Vector2.zero)
        {
            _playerPosRpc.OnMoveReleased(transform.position);
        }
    }

    public void SetMoveVector(Vector2 newVector)
    {
        _inputVec = newVector;
    }

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerPosRpc = GetComponent<PlayerPosRPC>();
        _playerAnimator = GetComponent<TheGamePlayerAnimator>();
    }

    void Start()
    {
        
        var inputHandler = GetComponent<TheGamePlayerInputHandler>();
        if (inputHandler != null)
        {
            Debug.Log("이벤트추가");
            inputHandler.OnMoveInput += HandleMoveInput;
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = _inputVec * _speed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + nextVec);
    }

    void OnDestroy()
    {
        var inputHandler = GetComponent<TheGamePlayerInputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnMoveInput -= HandleMoveInput;
        }
    }
}
