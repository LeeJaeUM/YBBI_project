using UnityEngine;
using UnityEngine.InputSystem;

public class Test_08_NavEnemy : TestBase
{
    public Transform target;
    public EnemyFSM_nav enemyFSM_nav;
    public override void Test1(InputAction.CallbackContext context)
    {
        enemyFSM_nav.Setup(target);
    }
}
