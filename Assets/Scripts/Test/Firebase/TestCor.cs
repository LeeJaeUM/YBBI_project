using UnityEngine;
using UnityEngine.InputSystem;

public class TestCor : TestBase
{
    public AuthUIManager authUIManager;
    public override void Test1(InputAction.CallbackContext context)
    {
        authUIManager.OperationFailed("test");
    }
}
