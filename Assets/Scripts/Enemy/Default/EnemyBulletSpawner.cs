using UnityEngine;

public class EnemyBulletSpawner : MonoBehaviour
{
    public void SpawnSingleBullet(
        bool isPlayer,
        SkillData skillData,
        float curAttackDamage,
        Vector3 spawnPosition,
        Vector3 direction)
    {
        Vector3 bulletSpawnPos = spawnPosition + direction.normalized * skillData._spawnDistance;

        GameObject bulletObj = BulletPool.Instance.GetBullet();
        bulletObj.transform.position = bulletSpawnPos;
        bulletObj.transform.rotation = Quaternion.identity;

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetArrowVector(direction);

            float damage = curAttackDamage * skillData._damageMultiplier;
            float radius = skillData._radius;
            float activeTime = skillData._activeTime;
            float speed = skillData._moveSpeed;
            // Sprite sprite = skillData._skillSprite; // 나중에 추가 가능
            Debug.Log($"총알 위치 싱글싱글 {spawnPosition}");
            bullet.SetData(isPlayer, damage, radius, skillData._width, skillData._length , activeTime, speed, skillData._bulletType);
        }
    }

    public void SpawnBulletSpread(
        bool isPlayer,
        SkillData skillData,   float curAttackDamage,
        Vector3 spawnPosition, Vector3 baseDirection)
    {
        float angleStep = (skillData._bulletCount > 1) ? skillData._angleRange / (skillData._bulletCount - 1) : 0;
        float startAngle = -skillData._angleRange / 2;

        for (int i = 0; i < skillData._bulletCount; i++)
        {
            float angleOffset = startAngle + i * angleStep;
            Vector3 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            Debug.Log($"총알 위치 {spawnPosition}");

            SpawnSingleBullet(isPlayer, skillData, curAttackDamage, spawnPosition, rotatedDirection);
        }
    }

}
