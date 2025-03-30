using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class PlayerPosRPC : NetworkBehaviour
{
    [SerializeField] private GameObject _textObj;

    private ulong _playerId;
    private GamePlayerInputHandler _player;
    //private TextPopup _textPopup;
    private void Start()
    {
        _player = GetComponent<GamePlayerInputHandler>();
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
        _player.SetMoveDir(deltaDir);
    }

    [ClientRpc]
    private void UpdateMovePosClientRpc(Vector2 deltaDir)
    {
        _player.SetMoveDir(deltaDir);
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

        _player.SetMoveDir(Vector2.zero);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopMoveServerRpc(Vector3 correctedPosition)
    {
        StopMoveClientRpc(correctedPosition);
        _player.SetMoveDir(Vector2.zero);
        transform.position = correctedPosition; // 보강
    }

    [ClientRpc]
    private void StopMoveClientRpc(Vector3 correctedPosition)
    {
        if (IsOwner) return;

        _player.SetMoveDir(Vector2.zero);
        transform.position = correctedPosition; // 클라이언트도 보강 (추가적으로)
    }

    //Skill_1******************************************************************************************************

/*    public void Skill_1Request(Vector2 spawnPosition)
    {
        InstanceText(OwnerClientId, spawnPosition);
        if (IsHost)
        {
            UpdateSkill_1ClientRpc(OwnerClientId, spawnPosition);
        }
        else if (IsClient)
        {
            UpdateSkill_1ServerRpc(OwnerClientId, spawnPosition);
        }
    }

    private void InstanceText(ulong playerId, Vector2 spawnPosition)
    {
        GameObject textPref = Instantiate(_textObj, spawnPosition, Quaternion.identity);
        TextPopup textpop = textPref.GetComponent<TextPopup>();
        textpop.SetId(playerId);
    }

    [ServerRpc]
    private void UpdateSkill_1ServerRpc(ulong playerId, Vector2 spawnPosition)
    {
        UpdateSkill_1ClientRpc(playerId, spawnPosition);
        InstanceText(playerId, spawnPosition);
    }

    [ClientRpc]
    private void UpdateSkill_1ClientRpc(ulong playerId, Vector2 spawnPosition)
    {
        if (IsOwner) return;
        InstanceText(playerId, spawnPosition);
    }*/
}