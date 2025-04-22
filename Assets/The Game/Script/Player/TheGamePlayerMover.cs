using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TheGamePlayerMover : NetworkBehaviour
{

    [SerializeField] private float _speed = 5f;

    private Tilemap[] allWallTilemaps;
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



    private bool IsWallAtPosition(Vector3 worldPosition)
    {
        foreach (var tilemap in allWallTilemaps)
        {
            Vector3Int cellPos = tilemap.WorldToCell(worldPosition);
            if (tilemap.GetTile(cellPos) != null)
            {
                return true; // 하나라도 있으면 막음
            }
        }
        return false;
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

        allWallTilemaps = FindObjectsOfType<Tilemap>();

        allWallTilemaps = System.Array.FindAll(allWallTilemaps, tm => tm.name.Contains("Wall"));
    }

    void FixedUpdate()
    {
        Vector2 nextVec = _inputVec * _speed * Time.fixedDeltaTime;
        Vector3 nextPos = _rigid.position + nextVec;

        if (!IsWallAtPosition(nextPos))
        {
            _rigid.MovePosition(nextPos);
        }
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
