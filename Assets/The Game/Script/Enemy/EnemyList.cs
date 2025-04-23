using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyList", menuName = "Enemy/EnemyList", order = 0)]
public class EnemyListSO : ScriptableObject
{
    public List<GameObject> Enemy;
}