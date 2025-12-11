using UnityEngine;
using UnityEngine.AI; // REQUIRED for NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10.0f; // How far to wander
    public float waitTime = 2.0f; // Time to stand still between points

    // Internal State
    private NavMeshAgent _agent;
    private float _waitTimer;
    private bool _isWaiting;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Disconnect rotation from physics to prevent jitter
        _agent.updateRotation = true;
        _agent.updateUpAxis = true;

        // Initialize first destination
        MoveToRandomPoint();
    }

    void Update()
    {
        // 1. Check if we reached the destination
        // pathPending: Is the computer still calculating the path?
        // remainingDistance: How far are we?
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            // 2. Wait Logic
            if (!_isWaiting)
            {
                _isWaiting = true;
                _waitTimer = 0f;
            }

            _waitTimer += Time.deltaTime;

            // 3. Pick New Point after timer finishes
            if (_waitTimer >= waitTime)
            {
                _isWaiting = false;
                MoveToRandomPoint();
            }
        }
    }

    void MoveToRandomPoint()
    {
        // LOGIC: Get a random point inside a sphere
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;

        // Offset logic: The sphere is centered on the NPC's current position
        randomDirection += transform.position;

        // NAVMESH SAMPLING:
        // We must ensure the random point is actually ON the blue walkable floor.
        // NavMesh.SamplePosition(SourcePoint, out HitResult, MaxDistance, AreaMask)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            // Apply the valid hit position to the Agent
            _agent.SetDestination(hit.position);
        }
    }
}