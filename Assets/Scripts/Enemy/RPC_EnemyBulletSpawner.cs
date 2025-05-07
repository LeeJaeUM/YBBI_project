using Unity.Netcode;
using UnityEngine;

public class RPC_EnemyBulletSpawner : NetworkBehaviour
{
    public GameObject _enemyBulletPrefab; // 적의 총알 프리팹
    public GameObject _playerBulletPrefab; // 플레이어의 총알 프리팹

    public SkillData[] _skillDatas; // 스킬 데이터 배열

    public void SpawnSingleBullet(
      SkillData skillData,
      float curAttackDamage,
      Vector3 spawnPosition,
      Vector3 direction)
    {
        if (!IsServer) return;

        Vector3 bulletSpawnPos = spawnPosition + direction.normalized * skillData._spawnDistance;

        // 서버에서만 총알을 생성
        GameObject bulletObj = BulletPool.Instance.GetBullet();
        bulletObj.transform.position = bulletSpawnPos;
        bulletObj.transform.rotation = Quaternion.identity;

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            float damage = curAttackDamage * skillData._damageMultiplier;
            float radius = skillData._radius;
            float activeTime = skillData._activeTime;
            float speed = skillData._moveSpeed;

            bullet.SetArrowVector(direction);
            bullet.SetData(damage, radius, skillData._width, skillData._length, activeTime, speed, skillData._bulletType);
        }

        // 클라이언트에 생성 요청
        //NotifySpawnSingleBulletClientRpc(
        //    skillData._ID,
        //    curAttackDamage,
        //    spawnPosition,
        //    direction
        //);
    }

    //[ClientRpc]
    //private void NotifySpawnSingleBulletClientRpc(
    //    int skillId,
    //    float curAttackDamage,
    //    Vector3 spawnPosition,
    //    Vector3 direction)
    //{
    //    if (IsServer) return;

    //    SkillData skillData = _skillDatas[skillId];
    //    if (skillData == null) return;

    //    Vector3 bulletSpawnPos = spawnPosition + direction.normalized * skillData._spawnDistance;

    //    GameObject bulletObj = BulletPool.Instance.GetBullet();
    //    bulletObj.transform.position = bulletSpawnPos;
    //    bulletObj.transform.rotation = Quaternion.identity;

    //    Bullet bullet = bulletObj.GetComponent<Bullet>();
    //    if (bullet != null)
    //    {
    //        float damage = curAttackDamage * skillData._damageMultiplier;
    //        float radius = skillData._radius;
    //        float activeTime = skillData._activeTime;
    //        float speed = skillData._moveSpeed;

    //        bullet.SetArrowVector(direction);
    //        bullet.SetData(damage, radius, skillData._width, skillData._length, activeTime, speed, skillData._bulletType);
    //    }
    //}

    public void SpawnBulletSpread(
        SkillData skillData,
        float curAttackDamage,
        Vector3 spawnPosition,
        Vector3 baseDirection)
    {
        if (!IsServer) return;

        float angleStep = (skillData._bulletCount > 1) ? skillData._angleRange / (skillData._bulletCount - 1) : 0;
        float startAngle = -skillData._angleRange / 2;

        for (int i = 0; i < skillData._bulletCount; i++)
        {
            float angleOffset = startAngle + i * angleStep;
            Vector3 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            SpawnSingleBullet(skillData, curAttackDamage, spawnPosition, rotatedDirection);
        }

        // 클라이언트에 스프레드 총알 생성 요청
        NotifySpawnBulletSpreadClientRpc(
            skillData._ID,
            curAttackDamage,
            spawnPosition,
            baseDirection
        );
    }

    [ClientRpc]
    private void NotifySpawnBulletSpreadClientRpc(
        int skillId,
        float curAttackDamage,
        Vector3 spawnPosition,
        Vector3 baseDirection)
    {
        if (IsServer) return;

        SkillData skillData = _skillDatas[skillId];
        if (skillData == null) return;

        float angleStep = (skillData._bulletCount > 1) ? skillData._angleRange / (skillData._bulletCount - 1) : 0;
        float startAngle = -skillData._angleRange / 2;

        for (int i = 0; i < skillData._bulletCount; i++)
        {
            float angleOffset = startAngle + i * angleStep;
            Vector3 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            Vector3 bulletSpawnPos = spawnPosition + rotatedDirection.normalized * skillData._spawnDistance;

            GameObject bulletObj = BulletPool.Instance.GetBullet();
            bulletObj.transform.position = bulletSpawnPos;
            bulletObj.transform.rotation = Quaternion.identity;

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                float damage = curAttackDamage * skillData._damageMultiplier;
                float radius = skillData._radius;
                float activeTime = skillData._activeTime;
                float speed = skillData._moveSpeed;

                bullet.SetArrowVector(rotatedDirection);
                bullet.SetData(damage, radius, skillData._width, skillData._length, activeTime, speed, skillData._bulletType);
            }
        }
    }
}
