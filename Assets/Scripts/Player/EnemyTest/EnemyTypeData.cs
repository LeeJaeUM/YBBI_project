using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypeData", menuName = "Scriptable Objects/EnemyTypeData")]
public class EnemyTypeData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;           // 적의 이름
    [Tooltip("사용할 Prefab")]
    public GameObject enemyPrefab;     // 인게임에서 소환할 적 프리팹

    [Header("능력치")]
    [Tooltip("최대 체력")]
    public int maxHealth;              // 최대 체력
    [Tooltip("이동 속도")]
    public float moveSpeed;            // 이동 속도
    [Tooltip("공격력")]
    public int attackPower;            // 공격력
    [Tooltip("방어력 (기본 - 0, 피해 감소를 사용할 경우 값 변경)")]
    public int defense;                // 방어력 (피해 감소에 활용)
    [Tooltip("공격 가능 거리")]
    public float attackRange;          // 공격 가능 거리
    [Tooltip("공격 주기 (초당 공격 횟수 혹은 클타임)")]
    public float attackSpeed;          // 공격 주기 
    [Tooltip("치명타 확률 (0 ~ 1 사이)")]
    public float criticalChance;       // 치명타 확률
    [Tooltip("치명타 배율 (예: 2.0 = 200%)")]
    public float criticalMultiplier;   // 치명타 배율

    [Header("AI 행동")]
    [Tooltip("플레이어를 인식 범위")]
    public float detectionRange;       // 플레이어를 인식하는 범위

    [Header("상태이상 및 면역")]
    [Tooltip("공격 시 독 상태 유발 여부")]
    public bool canApplyPoison;        // 공격 시 독 상태 유발 여부
    [Tooltip("공격 시 기절 유발 여부")]
    public bool canStun;               // 공격 시 기절 유발 여부

    //[Header("드랍 및 보상")]
    //public int expReward;              // 처치 시 주는 경험치

    //[Header("연출 및 이펙트")]
    //public GameObject deathEffect;     // 죽을 때 나오는 이펙트 프리팹
    //public GameObject hitEffect;       // 맞았을 때 나오는 이펙트 프리팹
    //public AudioClip spawnSound;       // 등장 사운드
    //public AudioClip deathSound;
}
