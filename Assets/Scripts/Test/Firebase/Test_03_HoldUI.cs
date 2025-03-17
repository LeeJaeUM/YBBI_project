using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Test_03_HoldUI : TestBase
{
    public override void Test1(InputAction.CallbackContext context)
    {
        Debug.Log($"{context.started}, {context.performed},  {context.canceled} ");
        if (context.started) Debug.Log("스탈트드");
        else if(context.performed) Debug.Log("펄폼드");
        else if(context.canceled) Debug.Log("캔슬드");
    }
    void TestFunc()
    {
        Debug.Log("누름");
    }
}
