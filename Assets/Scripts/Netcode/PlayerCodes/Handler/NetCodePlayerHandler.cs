using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class NetCodePlayerHandler : NetworkBehaviour
{
    [SerializeField] private GameObject _textObj;

    private ulong _playerId;
    private PlayerController _player;
    private TextPopup _textPopup;
    private void Start()
    {
        _player = GetComponent<PlayerController>();
        _textPopup = GetComponent<TextPopup>();
    }

    /*플레이어텔레포트******************************************************************************************************/
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
    }

    private void TeleportLocal(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [ServerRpc]
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

    /*플레이어이동******************************************************************************************************/

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

    /*Skill_1******************************************************************************************************/

    public void Skill_1Request(Vector2 spawnPosition)
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
    }
}