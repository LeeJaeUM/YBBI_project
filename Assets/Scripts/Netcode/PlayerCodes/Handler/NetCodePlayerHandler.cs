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

    /*플레이어이동******************************************************************************************************/

    public void MoveRequest(Vector2 deltaDir)
    {
        if (IsHost)
        {
            UpdatePositionClientRpc(deltaDir);
        }
        else if (IsClient)
        {
            UpdatePositionServerRpc(deltaDir);
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 deltaDir)
    {
        UpdatePositionClientRpc(deltaDir);
        _player.SetMoveDir(deltaDir);

    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector2 deltaDir)
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