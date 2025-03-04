using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCountroller : NetworkBehaviour
{
    [SerializeField] float _speed = 4.0f;

    private Vector2 moveDir;
    private PlayerHandler _playerHandler;


    public void OnMove(InputValue inputValue)
    {
        if (!IsOwner) { return; }

        moveDir = inputValue.Get<Vector2>();
        _playerHandler.MoveRequest(moveDir);
    }
    public void OnSkill_1()
    {
        if (!IsOwner) { return; }

        Debug.Log("Skill_1버튼 눌림");
        _playerHandler.Skill_1Request(OwnerClientId, transform.position);
    }

    public void SetMoveDir(Vector2 NewMoveDir)
    {
        moveDir = NewMoveDir;
    }

    private void Start()
    {
        _playerHandler = GetComponent<PlayerHandler>(); // 네트워크 핸들러 찾기
    }
    void Update()
    {
        transform.Translate(moveDir * _speed * Time.deltaTime);
    }
}