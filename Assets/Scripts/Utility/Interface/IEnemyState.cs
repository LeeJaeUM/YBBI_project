using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyAI enemy);
    void Execute(EnemyAI enemy);
    void Exit(EnemyAI enemy);
}
