using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class EnemyFSM_nav : MonoBehaviour
{
    [SerializeField]   private Transform target;
    [SerializeField]private NavMeshAgent navMeshAgent;
    [SerializeField] private BehaviorGraphAgent behaviorGraphAgent;

    public void Setup(Transform target)
    {
        this.target = target;

        navMeshAgent = GetComponent<NavMeshAgent>();
        behaviorGraphAgent= GetComponent<BehaviorGraphAgent>();
        navMeshAgent.updateRotation = false;    //자동 회전 방지
        navMeshAgent.updateUpAxis = false;      //자동 회전 방지
    }

    private void Awake()
    {
        Setup(target);
    }
}
