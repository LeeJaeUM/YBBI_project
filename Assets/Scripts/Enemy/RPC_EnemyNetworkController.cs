using UnityEngine;
using Unity.Netcode;

public class RPC_EnemyNetworkController : NetworkBehaviour
{
    private bool isHost = false;
    private bool isOwner = false;

    public bool GetIsHost()
    {
        return isHost;
    }
    public bool GetIsOwner()
    {
        return isOwner;
    }
    void Initialized()
    { 
        isHost = NetworkManager.Singleton.IsHost;
        isOwner = IsOwner;
    }

    private void Awake()
    {
        Initialized();
    }
}
