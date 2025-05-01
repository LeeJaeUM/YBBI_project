using UnityEngine;
using UnityEngine.InputSystem;

public class Test_09_EnemyAnimation : TestBase
{
    public Animator animator;

    public string animName1 = "IsWalk";
    public string animName2 = "IsInteract";
    public string animName3 = "Attack_1";
    public string animName4 = "Hurt";
    public string animName5 = "Die";
    public bool isTrue = true;

    public override void Test1(InputAction.CallbackContext context)
    {
        animator.SetBool(animName1, isTrue);
        isTrue = !isTrue;
    }
    public override void Test2(InputAction.CallbackContext context)
    {
        animator.SetBool(animName2, isTrue);
        isTrue = !isTrue;
    }

    public override void Test3(InputAction.CallbackContext context)
    {
        animator.SetTrigger(animName3);
    }

    public override void Test4(InputAction.CallbackContext context)
    {
        animator.SetTrigger(animName4);
    }

    public override void Test5(InputAction.CallbackContext context)
    {
        animator.SetTrigger(animName5);
    }

}
