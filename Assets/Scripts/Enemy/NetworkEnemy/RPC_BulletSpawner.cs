using Unity.Netcode;
using UnityEngine;

public class RPC_BulletSpawner : NetworkBehaviour
{
    //public GameObject _enemyBulletPrefab; // 적의 총알 프리팹
    //public GameObject _playerBulletPrefab; // 플레이어의 총알 프리팹

    public SkillData[] _skillDatas; // 스킬 데이터 배열

    /// <summary>
    /// 각자 자기 자신의 클라에서 총알을 발사하는 메서드
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="skillData"></param>
    /// <param name="curAttackDamage"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="direction"></param>
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
            float damage = curAttackDamage * skillData._damageMultiplier;
            float radius = skillData._radius;
            float activeTime = skillData._activeTime;
            float speed = skillData._moveSpeed;

            bullet.SetArrowVector(direction);
            bullet.SetData(isPlayer, damage, radius, skillData._width, skillData._length, activeTime, speed, skillData._bulletType);
        }
    }


    /// <summary>
    /// 몬스터나 플레이어가 요청하는 총알 발사 메서드. 부른 후 서버인지 클라인지 판단
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="skillData"></param>
    /// <param name="curAttackDamage"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="baseDirection"></param>
    public void RequestSpawnBulletSpread(
    bool isPlayer,
    SkillData skillData,
    float curAttackDamage,
    Vector3 spawnPosition,
    Vector3 baseDirection)
    {
        if (IsServer)
        {
            SpawnBulletSpread(isPlayer, skillData, curAttackDamage, spawnPosition, baseDirection);
        }
        else
        {
            RequestSpawnBulletSpreadServerRpc(isPlayer, skillData._ID, curAttackDamage, spawnPosition, baseDirection);
        }
    }

    [ServerRpc]
    private void RequestSpawnBulletSpreadServerRpc(
    bool isPlayer,
    int skillId,
    float curAttackDamage,
    Vector3 spawnPosition,
    Vector3 baseDirection)
    {
        SkillData skillData = _skillDatas[skillId];
        if (skillData == null) return;

        SpawnBulletSpread(isPlayer, skillData, curAttackDamage, spawnPosition, baseDirection);
    }

    /// <summary>
    /// 서버에서 총알을 발사하는 메서드
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="skillData"></param>
    /// <param name="curAttackDamage"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="baseDirection"></param>
    private void SpawnBulletSpread(
        bool isPlayer,
        SkillData skillData,
        float curAttackDamage,
        Vector3 spawnPosition,
        Vector3 baseDirection)
    {
        ExecuteBulletSpread(isPlayer, skillData, curAttackDamage, spawnPosition, baseDirection);

        NotifySpawnBulletSpreadClientRpc(
            isPlayer,
            skillData._ID,
            curAttackDamage,
            spawnPosition,
            baseDirection
        );
    }

    [ClientRpc]
    private void NotifySpawnBulletSpreadClientRpc(
        bool isPlayer,
        int skillId,
        float curAttackDamage,
        Vector3 spawnPosition,
        Vector3 baseDirection)
    {
        if (IsServer) return;

        SkillData skillData = _skillDatas[skillId];
        if (skillData == null) return;

        ExecuteBulletSpread(isPlayer, skillData, curAttackDamage, spawnPosition, baseDirection);
    }

    /// <summary>
    /// 실제 총알을 발사하는 메서드
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="skillData"></param>
    /// <param name="curAttackDamage"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="baseDirection"></param>
    private void ExecuteBulletSpread(
    bool isPlayer,
    SkillData skillData,
    float curAttackDamage,
    Vector3 spawnPosition,
    Vector3 baseDirection)
    {
        float angleStep = (skillData._bulletCount > 1) ? skillData._angleRange / (skillData._bulletCount - 1) : 0;
        float startAngle = -skillData._angleRange / 2;

        for (int i = 0; i < skillData._bulletCount; i++)
        {
            float angleOffset = startAngle + i * angleStep;
            Vector3 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            SpawnSingleBullet(isPlayer, skillData, curAttackDamage, spawnPosition, rotatedDirection);
        }
    }
}
