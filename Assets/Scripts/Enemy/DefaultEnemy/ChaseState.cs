using UnityEngine;

public class ChaseState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} started chasing the player!");

    public void Execute(EnemyAI enemy)
    {
        if (enemy.Player == null) return;
        Vector2 _dir = (enemy.Player.position - enemy.transform.position).normalized;
        enemy.transform.Translate(_dir * enemy._speed * Time.deltaTime);
    }

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} stopped chasing.");
}
