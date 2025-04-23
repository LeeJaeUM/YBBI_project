using UnityEngine;

public class EnemyFSM_nav : MonoBehaviour
{
    private Transform target;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    public void Setup(Transform target)
    {
        this.target = target;

        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.updateRotation = false;    //자동 회전 방지
        navMeshAgent.updateUpAxis = false;      //자동 회전 방지
    }

    private void Update()
    {
        navMeshAgent.SetDestination(target.position);
    }
}
