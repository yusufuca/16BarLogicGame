using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Attack }

    [Header("Stats")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5.5f;
    public float detectionRadius = 10f; // Range to start chasing
    public float attackRadius = 1.5f;   // Range to stop and hit

    [Header("Patrol Settings")]
    public float patrolRadius = 10.0f;
    public float waitTime = 2.0f;

    // References
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private State _currentState;

    // Internal Timers
    private float _waitTimer;
    private float _attackCooldown = 1.5f; // Time between hits
    private float _lastAttackTime;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        // Disconnect rotation from physics
        _agent.updateRotation = true;
        _agent.updateUpAxis = true;

        // Auto-find Player by Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: Could not find object with Tag 'Player'!");
        }

        // Initialize
        _currentState = State.Patrol;
        MoveToRandomPoint();
    }

    void Update()
    {
        if (_player == null) return;

        // 1. Calculate Distance
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // 2. State Switching Logic
        if (distanceToPlayer <= attackRadius)
        {
            _currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            _currentState = State.Chase;
        }
        else
        {
            _currentState = State.Patrol;
        }

        // 3. Execute State Behavior
        switch (_currentState)
        {
            case State.Patrol:
                PatrolBehavior();
                break;
            case State.Chase:
                string requestedState = "Combat";
                ChaseBehavior();
                AudioManager.AMInstance.RequestState(requestedState);
                break;
            case State.Attack:
                AttackBehavior();
                break;
        }

        // 4. Update Animator Locomotion
        // Pass the Agent's velocity to the Blend Tree (assuming you reused the Player's controller)
        // If not, remove this line or adapt to your specific NPC animator.
        float speed = _agent.velocity.magnitude / chaseSpeed;
        _animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    void PatrolBehavior()
    {
        _agent.speed = patrolSpeed;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitTime)
            {
                MoveToRandomPoint();
                _waitTimer = 0f;
            }
        }
    }

    void ChaseBehavior()
    {
       
        _agent.speed = chaseSpeed;
        _agent.SetDestination(_player.position); // Update path to Player constantly
       
    }

    void AttackBehavior()
    {
        // Stop moving
        _agent.ResetPath();

        // Face the player
        transform.LookAt(_player);

        // Attack Cooldown Logic
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _animator.SetTrigger("Attack");
            _lastAttackTime = Time.time;

            // Debug Log for now (Damage logic comes later)
            CharacterStats targetStats = _player.GetComponent<CharacterStats>();
            if (targetStats != null)
            {
                targetStats.TakeDamage(10); // Enemy deals 10 damage
            }
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            _agent.SetDestination(hit.position);
        }
    }

    public void OnStep()
    {
        AudioManager.AMInstance.DetectSurface(transform.root);
    }
}