using Unity.Netcode;
using UnityEngine;

public class TestNetTeamPicker : MonoBehaviour
{

    public void SetTeam(int teamIndex)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if(!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient client)) { return; }

        if(!client.PlayerObject.TryGetComponent<TestNetTeamPlayer>(out var teamPlayer)) { return; }

        teamPlayer.SetTeamServerRpc((byte)teamIndex); 
    }
}
