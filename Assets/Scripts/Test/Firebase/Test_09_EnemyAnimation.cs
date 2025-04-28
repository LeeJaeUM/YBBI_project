using UnityEngine;
using UnityEngine.InputSystem;

public class Test_09_EnemyAnimation : TestBase
{
    public Animator animator;

    public override void Test3(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack_1");
    }

    public override void Test4(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack_2");
    }

    public override void Test5(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Teleport");
    }

    public override void Test6(InputAction.CallbackContext context)
    {
        animator.SetTrigger("PhaseChange");
    }
}
