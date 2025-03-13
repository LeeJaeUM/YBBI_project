using System.Net;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public static PlayerData LocalInstance { get; private set; }

    public NetworkVariable<string> PlayerID = new NetworkVariable<string>();
    public NetworkVariable<bool> IsReady = new NetworkVariable<bool>();

    private void Awake()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }

    private void Start()
    {
        PlayerID.Value = OwnerClientId.ToString();
        IsReady.Value = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetPlayerIDServerRpc(OwnerClientId.ToString()); // 플레이어 ID 설정
        }
    }

    [ServerRpc]
    public void SetPlayerIDServerRpc(string id)
    {
        PlayerID.Value = id;
    }

    [ServerRpc]
    public void ToggleReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
    }

    [ClientRpc]
    public void UpdateReadyStateClientRpc(bool ready)
    {
        IsReady.Value = ready;
    }
}