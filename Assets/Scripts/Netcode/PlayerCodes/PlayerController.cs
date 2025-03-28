using Unity.Netcode;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float _speed = 4.0f;

    private Vector2 _moveDir;
    private NetCodePlayerHandler _playerHandler;

    
    public void OnMove(InputValue inputValue)
    {
        if (!IsOwner) { return; }

        Vector2 input = inputValue.Get<Vector2>();
        _moveDir = input;
        _playerHandler.MoveRequest(_moveDir);

        if (input == Vector2.zero)
        {
            // 이동키 뗐을 때 → 위치 보강
            _playerHandler.OnMoveReleased(transform.position);
        }
    }
    public void OnSkill_1()
    {
        if (!IsOwner) { return; }

        Debug.Log("Skill_1버튼 눌림");
        _playerHandler.Skill_1Request( transform.position);
    }

    public void SetMoveDir(Vector2 NewMoveDir)
    {
        _moveDir = NewMoveDir;
    }

    private void Start()
    {
        _playerHandler = GetComponent<NetCodePlayerHandler>();


    }

    void Update()
    {
        transform.Translate(_moveDir * _speed * Time.deltaTime);
    }
}