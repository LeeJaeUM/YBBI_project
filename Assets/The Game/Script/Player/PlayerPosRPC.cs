using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerPosRPC : NetworkBehaviour
{
    //[SerializeField] private GameObject _textObj;

    //private ulong _playerId;
    private TheGamePlayerMover _player;
    private bool isTP = false;

    //private TextPopup _textPopup;
    private void Start()
    {
        _player = GetComponent<TheGamePlayerMover>();
        //_textPopup = GetComponent<TextPopup>();
    }
    //플레이어위치보강******************************************************************************************************

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc(Vector3 correctedPosition)
    {
        // 서버에서 위치를 다시 한 번 보정
        transform.position = correctedPosition;
    }

    //플레이어텔레포트******************************************************************************************************
    public void TeleportRequest(Vector3 newPosition)
    {
        if (isTP) return;

        if (IsHost)
        {
            TeleportClientRpc(newPosition);
        }
        else if (IsClient)
        {
            TeleportServerRpc(newPosition);
        }

        TeleportLocal(newPosition); // 로컬도 이동

        UpdatePositionServerRpc(newPosition);

        StartCoroutine(TeleportCooldown());

    }

    public bool IsTeleporting()
    {
        return isTP;
    }

    private IEnumerator TeleportCooldown()
    {
        isTP = true;
        yield return new WaitForSeconds(1f);
        isTP = false;
    }

    private void TeleportLocal(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportServerRpc(Vector3 newPosition)
    {
        TeleportClientRpc(newPosition);
        TeleportLocal(newPosition); // 서버에서도 이동
    }

    [ClientRpc]
    private void TeleportClientRpc(Vector3 newPosition)
    {
        if (IsOwner) return; // 본인은 이미 이동했음
        TeleportLocal(newPosition);
    }
    //플레이어이동******************************************************************************************************

    public void MoveRequest(Vector2 deltaDir)
    {
        if (IsHost)
        {
            UpdateMovePosClientRpc(deltaDir);
        }
        else if (IsClient)
        {
            UpdateMovePosServerRpc(deltaDir);
        }
    }

    [ServerRpc]
    private void UpdateMovePosServerRpc(Vector2 deltaDir)
    {
        UpdateMovePosClientRpc(deltaDir);
        _player.SetMoveVector(deltaDir);
    }

    [ClientRpc]
    private void UpdateMovePosClientRpc(Vector2 deltaDir)
    {
        _player.SetMoveVector(deltaDir);
    }

    public void OnMoveReleased(Vector3 currentPosition)
    {
        if (IsHost)
        {
            StopMoveClientRpc(currentPosition);
        }
        else
        {
            StopMoveServerRpc(currentPosition);
        }

        _player.SetMoveVector(Vector2.zero);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopMoveServerRpc(Vector3 correctedPosition)
    {
        StopMoveClientRpc(correctedPosition);
        _player.SetMoveVector(Vector2.zero);
        transform.position = correctedPosition; // 보강
    }

    [ClientRpc]
    private void StopMoveClientRpc(Vector3 correctedPosition)
    {
        if (IsOwner) return;

        _player.SetMoveVector(Vector2.zero);
        transform.position = correctedPosition; // 클라이언트도 보강 (추가적으로)
    }
}