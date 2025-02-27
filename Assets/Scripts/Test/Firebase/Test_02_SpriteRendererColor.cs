using UnityEngine;
using UnityEngine.InputSystem;

public class Test_02_SpriteRendererColor : TestBase
{
    public SpriteRenderer _renderer;
    public override void Test1(InputAction.CallbackContext context)
    {
        _renderer.color = Color.blue;
    }
}
