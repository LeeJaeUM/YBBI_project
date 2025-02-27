using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestNetPlayer : NetworkBehaviour
{
    [SerializeField]private float _speed = 5.0f;
    private Vector2 _moveInput;
    private void OnMove(InputValue value)
    {
        if (!IsOwner) return;

        _moveInput = value.Get<Vector2>();

        if(IsHost)
        {
            MoveClientRpc(_moveInput);
        }
        else if(IsClient)
        {
            MoveServerRpc(_moveInput);
        }
    }
    private void Update()
    {
        //if(!IsOwner) return;
        Move(_moveInput);
    }

    private void Move(Vector2 input)
    {
        // 입력 값이 있을 때만 이동
        if (input != Vector2.zero)
        {
            transform.position += (Vector3)(input * _speed * Time.deltaTime);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 inputVec)
    { 
        _moveInput = inputVec;
        MoveClientRpc(inputVec);
    }

    [ClientRpc]
    private void MoveClientRpc(Vector2 inputVec)
    {
        _moveInput = inputVec;
    }

}