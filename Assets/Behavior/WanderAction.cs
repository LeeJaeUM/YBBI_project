using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[Self] Navigate to WanderPosition", category: "Action", id: "1e7d005d9a73051df861024661ed3d38")]
public partial class WanderAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private NavMeshAgent _agent; 
    private Vector3 _wanderPosition;
    private float _currentWanderTime = 0.0f;
    private float _macWanderTime = 5.0f;

    protected override Status OnStart()
    {
        int jitterMin = 0;
        int jitterMax = 360;
        float wanderRadius = UnityEngine.Random.Range(1.5f, 4.5f);
        int wanderJitter = UnityEngine.Random.Range(jitterMin, jitterMax);

        // 목표 위치 = 자신의 위치 + (방향 * 반지름)
        _wanderPosition = Self.Value.transform.position + GetPositionFromAngle(wanderRadius, wanderJitter);
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.SetDestination(_wanderPosition);
        _currentWanderTime = Time.time;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }

    private Vector3 GetPositionFromAngle(float angle, float radius)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        return new Vector3(x, 0, z);
    }
}

