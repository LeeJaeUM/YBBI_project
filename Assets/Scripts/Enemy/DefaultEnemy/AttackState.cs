using UnityEngine;

public class AttackState : IEnemyState
{
    public void Enter(EnemyAI enemy) => Debug.Log($"{enemy.name} is attacking!");

    public void Execute(EnemyAI enemy) => Debug.Log($"{enemy.name} attacks!");

    public void Exit(EnemyAI enemy) => Debug.Log($"{enemy.name} stopped attacking.");
}