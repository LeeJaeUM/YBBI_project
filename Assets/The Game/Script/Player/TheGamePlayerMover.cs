using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class TheGamePlayerMover : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;

    private Tilemap[] allWallTilemaps;
    private Rigidbody2D _rigid;
    private Vector2 _inputVec;
    private PlayerPosRPC _playerPosRpc;
    private TheGamePlayerAnimator _playerAnimator;
    private SetCameraTarget _cameraTarget;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateWallTilemaps();
        _cameraTarget.SetTarget();

        var inputHandler = GetComponent<TheGamePlayerInputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnMoveInput -= HandleMoveInput; // 중복 방지
            inputHandler.OnMoveInput += HandleMoveInput;
        }
    }

    private void UpdateWallTilemaps()
    {
        allWallTilemaps = FindObjectsOfType<Tilemap>();
        allWallTilemaps = System.Array.FindAll(allWallTilemaps, tm => tm.name.Contains("Wall"));

        if (allWallTilemaps.Length == 0)
        {
            Debug.LogWarning("[PlayerMover] Wall 타일맵을 찾지 못했습니다.");
        }
    }

    public void ForceUpdateWallTilemaps()
    {
        Debug.Log("[PlayerMover] 맵 생성 완료 후 타일맵 갱신 요청됨");
        UpdateWallTilemaps();
    }

    private void HandleMoveInput(Vector2 input)
    {
        if (!IsLocalPlayer) return;

        _inputVec = input;
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
        Vector2 offset = new Vector2(0.5f, 0.5f); // 충돌 감지 영역 크기
        Vector3 topLeft = worldPosition + (Vector3)(-offset);
        Vector3 bottomRight = worldPosition + (Vector3)(offset);

        for (float x = topLeft.x; x <= bottomRight.x; x += 0.1f)
        {
            for (float y = topLeft.y; y <= bottomRight.y; y += 0.1f)
            {
                foreach (var tilemap in allWallTilemaps)
                {
                    if (tilemap == null) continue; // null 방지

                    Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
                    if (tilemap.HasTile(cellPos))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (allWallTilemaps == null || allWallTilemaps.Length == 0)
        {
            Debug.Log("벽타일이 없음");
            return; // 타일맵이 아직 준비되지 않은 경우 이동 무시
        }

        Vector2 nextVec = _inputVec * _speed * Time.fixedDeltaTime;
        Vector3 nextPos = _rigid.position + nextVec;

        if (!IsWallAtPosition(nextPos))
        {
            _rigid.MovePosition(nextPos);
        }
    }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerPosRpc = GetComponent<PlayerPosRPC>();
        _playerAnimator = GetComponent<TheGamePlayerAnimator>();
        _cameraTarget = GetComponent<SetCameraTarget>();
    }

    private void Start()
    {
        var inputHandler = GetComponent<TheGamePlayerInputHandler>();
        if (inputHandler != null)
        {
            Debug.Log("이벤트추가");
            inputHandler.OnMoveInput += HandleMoveInput;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        _cameraTarget.SetTarget();
        UpdateWallTilemaps(); // 최초 씬에서도 탐색
    }

    private void OnDestroy()
    {
        var inputHandler = GetComponent<TheGamePlayerInputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnMoveInput -= HandleMoveInput;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
