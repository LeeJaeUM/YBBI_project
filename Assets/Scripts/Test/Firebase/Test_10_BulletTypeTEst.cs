using UnityEngine;
using UnityEngine.InputSystem;

public class Test_10_BulletTypeTEst : TestBase
{
    public SkillData skillData;
    public Vector2 right = Vector2.right;
    public GameObject bulletPrefab;

    public override void Test1(InputAction.CallbackContext context)
    {

        GameObject bulletObj = BulletPool.Instance.GetBullet();
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetArrowVector(right);
        // bulletObj.transform.position = bulletSpawnPos;
        bulletObj.transform.rotation = Quaternion.identity;

        float damage = 1 * skillData._damageMultiplier;
        float radius = skillData._radius;
        float activeTime = skillData._activeTime;
        float speed = skillData._moveSpeed;
        bullet.SetData(damage, radius, skillData._width, skillData._length, activeTime, speed, skillData._bulletType);
    }
}

