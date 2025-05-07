using UnityEngine;
using Unity.Netcode;
using static Enums;

public class RPC_EnemyAI : EnemyAI
{
    public RPC_EnemyNetworkController _enemyNetworkController;
    public bool isHost = false;
    public bool isOwner = false;

    public override void SetRandomAttackPatern()
    {
        int randomIndex = Random.Range(0, _maxPaternNumber);  // 1 ~ max-1 랜덤 값0
#if UNITY_EDITOR
        if (TEST_isSkillSet) //에디터에서만 사용
        {
            randomIndex = TEST_skillNum; //에디터에서만 사용
        }
#endif
        _curATKPatern = (ATKPatern)randomIndex;
        _enemyATKStats.SetPaaternNum(randomIndex);

    }
    public override void StartAttack()
    {
        Vector3 direction = _findTargetPoint.GetTargetDirection();

        SetFlipX(direction.x < 0); //플레이어 방향으로 스프라이트 반전

        _enemyAniamtor.UpdateMoveVisual(direction);        //적의 바라보는 방향에 따라 애니메이션을 업데이트
        _enemyAniamtor.PlayAttackAnimation();

        _enemyATKStats.Attack(direction);
    }

    public override void EnemyMove(Vector2 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
        SetFlipX(direction.x < 0);
    }
    public override void SetFlipX(bool isFlip)
    {
        if (_spriteRenderer.flipX == isFlip) return; //스프라이트 반전이 필요없으면 리턴

        _spriteRenderer.flipX = isFlip; //스프라이트 반전
    }

    #region Unity Built-in Fuction  
    protected override void Start()
    {
        base.Start();
        _enemyNetworkController = GetComponent<RPC_EnemyNetworkController>();
        isHost = _enemyNetworkController.GetIsHost();
        isOwner = _enemyNetworkController.IsOwner;
    }

    protected override void Update()
    {
        if(!isOwner) return; 
        if (isHost)
        {
            // Host-specific logic
            base.Update();
        }
        else
        {
            // Client-specific logic
            // 현재 클라이언트는 상태를 업데이트하지 않음
        }
    }
    #endregion
}
