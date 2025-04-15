using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Type Data")]
    public EnemyTypeData typeData; // ScriptableObject에서 적 스탯 참조

    // 실제 인게임에서 사용할 능력치
    private string enemyName;
    private int currentHealth;
    private int attackPower;
    private int defense;
    private float moveSpeed;
    private float attackRange;
    private float attackSpeed;
    private float criticalChance;
    private float criticalMultiplier;
    private float detectionRange;

    private bool canApplyPoison;
    private bool canStun;

    void Start()
    {
        if (typeData == null)
        {
            Debug.LogError("EnemyTypeData is not assigned!", this);
            return;
        }

        // 타입 데이터에서 초기화
        enemyName = typeData.enemyName;
        currentHealth = typeData.maxHealth;
        attackPower = typeData.attackPower;
        defense = typeData.defense;
        moveSpeed = typeData.moveSpeed;
        attackRange = typeData.attackRange;
        attackSpeed = typeData.attackSpeed;
        criticalChance = typeData.criticalChance;
        criticalMultiplier = typeData.criticalMultiplier;
        detectionRange = typeData.detectionRange;

        canApplyPoison = typeData.canApplyPoison;
        canStun = typeData.canStun;

        Debug.Log($"{enemyName} spawned with {currentHealth} HP | ATK: {attackPower}, DEF: {defense}, SPD: {moveSpeed}");
    }

    public void TakeDamage(int damage)
    {
        // 방어력 적용한 데미지 계산 (최소 1)
        int effectiveDamage = Mathf.Max(1, damage - defense);
        currentHealth -= effectiveDamage;

        Debug.Log($"{enemyName} took {effectiveDamage} damage (raw: {damage}, DEF: {defense}). Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyName} died.");
        //ItemManager.Instance.Create(transform.position); // 드랍 아이템 생성
        Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(5);
        }
    }
}

