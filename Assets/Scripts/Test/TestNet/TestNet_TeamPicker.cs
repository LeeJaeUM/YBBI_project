using Unity.Netcode;
using UnityEngine;

public class TestNet_TeamPicker : MonoBehaviour
{

    public void SetTeam(int teamIndex)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if(!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient client)) { return; }

        if(!client.PlayerObject.TryGetComponent<TestNet_TeamPlayer>(out var teamPlayer)) { return; }

        teamPlayer.SetTeamServerRpc((byte)teamIndex); 
    }
}
