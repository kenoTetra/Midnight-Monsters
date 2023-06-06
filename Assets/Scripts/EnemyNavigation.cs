using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    // The NavMeshAgent attached to this GameObject
    [SerializeField]
    private NavMeshAgent agent;

    // The target to follow; set to null to use targetPosition instead
    public Transform target;

    // targetPosition is used if target is null
    public Vector3 targetPosition;

    // The distance from the target that the object will stop
    public float stoppingDistance = 10f;

    // Whether or not the object should face the direction it is moving
    public bool faceDirection = true;

    // The remaining distance to the target
    public float remainingDistance = float.MaxValue;

    void Awake()
    {
        // If agent is not set, try to get it from the GameObject
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Start()
    {
        UpdateAgentMembers();
    }

    void Update()
    {
        UpdateAgentMembers();
    }

    public void UpdateAgentMembers()
    {
        if (agent == null)
        {
            return;
        }

        agent.destination = target?.position ?? targetPosition;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = faceDirection;
        remainingDistance = agent.remainingDistance;
    }
}
