using UnityEngine;
using UnityEngine.InputSystem;

public class Test_09_EnemyAnimation : TestBase
{
    public Animator animator;

    public string animName1 = "IsWalk";
    public string animName2 = "IsAttack";
    public string animName3 = "IsHurt";
    public string animName4 = "IsDead";
    public bool isTrue = true;

    public override void Test3(InputAction.CallbackContext context)
    {
        animator.SetBool(animName1, isTrue);
        isTrue = !isTrue;
    }

    public override void Test4(InputAction.CallbackContext context)
    {
        animator.SetBool(animName2, isTrue);
        isTrue = !isTrue;
    }

    public override void Test5(InputAction.CallbackContext context)
    {
        animator.SetBool(animName3, isTrue);
        isTrue = !isTrue;
    }

    public override void Test6(InputAction.CallbackContext context)
    {
        animator.SetBool(animName4, isTrue);
        isTrue = !isTrue;
    }
}
