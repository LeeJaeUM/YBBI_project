using UnityEngine;
using UnityEngine.InputSystem;

public class Test_07_BulletSpawner : TestBase
{
    public EnemyBulletSpawner _enemyBulletSpawner;
    public Vector3 _direction = Vector3.right;
    public float _angleRange = 45f;
    public int _bulletCount = 5;
    public SkillData skillData;
    public float curAttackDamage = 2;


    public LaserWarningVisualizer _laserWarningVisualizer ;
    public override void Test1(InputAction.CallbackContext context)
    {
        _enemyBulletSpawner.SpawnBulletSpread(skillData, curAttackDamage, transform.position, _direction);   
    }

    public override void Test2(InputAction.CallbackContext context)
    {
        _laserWarningVisualizer.ShowLaserWarning(transform.position, _direction, 0.5f, 5f, 3f);
    }
}
