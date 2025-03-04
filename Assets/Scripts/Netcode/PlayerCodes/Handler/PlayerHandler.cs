using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class PlayerHandler : NetworkBehaviour
{
    [SerializeField] private GameObject _textObj;

    public static ulong _staticPlayerId;
    private PlayerCountroller _player;
    private TextPopup _textPopup;
    private void Start()
    {
        _player = GetComponent<PlayerCountroller>();
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

    public void Skill_1Request(ulong playerId,Vector2 spawnPosition)
    {
        _staticPlayerId = playerId;
        UpdateSkill_1ServerRpc(playerId, spawnPosition);
        Instantiate(_textObj, spawnPosition, Quaternion.identity);
        Debug.Log("skill_1Request에서텍스트 생성");

    }

    [ServerRpc]
    private void UpdateSkill_1ServerRpc(ulong playerId, Vector2 spawnPosition)
    {
        UpdateSkill_1ClientRpc(playerId, spawnPosition);
    }

    [ClientRpc]
    private void UpdateSkill_1ClientRpc(ulong playerId, Vector2 spawnPosition)
    {
        if (IsOwner) return;
        _staticPlayerId = playerId;
        Instantiate(_textObj, spawnPosition, Quaternion.identity);
        Debug.Log("ClinetRpc에서텍스트 생성");
    }
}