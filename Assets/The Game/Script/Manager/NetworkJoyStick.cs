using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class NetworkJoyStick : NetworkBehaviour
{
    OnScreenStick _screenStick;
    private void Awake()
    {
        _screenStick = GetComponent<OnScreenStick>();
    }

    private void TestStick(bool value)
    {
        if (_screenStick != null)
        {
            _screenStick.enabled = value;
        }
    }

    public void SetInput(TheGamePlayerInputHandler input)
    {
        if (!IsLocalPlayer)
        {
            // 입력 관련 컴포넌트 비활성화
            GetComponent<TheGamePlayerInputHandler>().enabled = false;
        }
        else
        {
            // 조이스틱 UI에 연결 (예: OnScreenStick의 방향값을 여기에 연결)
            
        }
    }
}
