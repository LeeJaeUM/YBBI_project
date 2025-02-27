using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetUI : MonoBehaviour
{
    public Button _startHostBtn;
    public Button _joinClientBtn;
    public Button _leaveRoomBtn;

    private void Start()
    {
        _startHostBtn.onClick.AddListener(OnStartHostBtn);
        _joinClientBtn.onClick.AddListener(OnJoinClientBtn);
        _leaveRoomBtn.onClick.AddListener(OnLeaveRoomBtn);
    }

    private void OnStartHostBtn()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnJoinClientBtn()
    {
        NetworkManager.Singleton.StartClient(); 
    }

    private void OnLeaveRoomBtn()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
