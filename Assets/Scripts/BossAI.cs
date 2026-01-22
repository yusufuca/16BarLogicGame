using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class BossAI : MonoBehaviour
{
    private enum BossState { Idle, Chasing, Attacking }
    private BossState currentState;

    [Header("Stats")]
    public float detectionRange = 20f;
    public float attackRange = 10f; // Ranged boss
    public float moveSpeed = 0;

    [Header("Phase 2")]
    public bool isPhase2 = false;
    public float phase2SpeedMultiplier = 1.5f;
    public Material phase2Material; // Visual change (optional)

    [Header("Abilities")]
    public GameObject poisonPrefab;
    public Transform firePoint; // Where projectiles come out
    public float abilityCooldown = 3.0f; // Time between attacks

    private NavMeshAgent _agent;
    private Transform _player;
    private CharacterStats _myStats;
    private float _nextAbilityTime;

    public float patrolSpeed = 0f;
    public float chaseSpeed = 5.5f;
    private float _waitTimer;
    public float waitTime = 2.0f;
    public float patrolRadius = 10.0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myStats = GetComponent<CharacterStats>();
        
        _agent.speed = patrolSpeed;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;
    }

    void Update()
    {
        if (_player == null || _myStats.currentHealth <= 0) return;
        float dist = Vector3.Distance(transform.position, _player.position);


        if (dist <= attackRange)
        {
            currentState = BossState.Attacking;
        }
        else if (dist <= detectionRange)
        {
            currentState = BossState.Chasing;
        }
        else
        {
            currentState = BossState.Idle;
        }

        switch (currentState)
        {
            case BossState.Idle:
                PatrolBehavior();
                break;
            case BossState.Chasing:
                ChaseBehavior();
                break;
            case BossState.Attacking:
                Attacking();
                break;
        }

        Debug.Log($"Boss State is {currentState}");

        // Check Phase 2 Logic
        CheckPhase();


    }
   
    void Attacking()
    {
       

            currentState = BossState.Attacking;
            _agent.ResetPath();
            transform.LookAt(_player);

            if (Time.time >= _nextAbilityTime)
            {
                PerformRandomAbility();
                _nextAbilityTime = Time.time + abilityCooldown;
            }
        
      
    }
    void CheckPhase()
    {
        // GDD: Phase 2 increases speed/damage 
        if (!isPhase2 && _myStats.currentHealth < (_myStats.maxHealth / 2))
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
       
        abilityCooldown *= 0.6f; // Attack faster

        // Visual Feedback: Change color to Red
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null && phase2Material != null) rend.material = phase2Material;

        Debug.Log("BOSS ENTERED PHASE 2!");
    }

    void PerformRandomAbility()
    {
        int rand = Random.Range(0, 3); // 0, 1, or 2

        switch (rand)
        {
            case 0:
                ThrowPoison();
                break;
            case 1:
                RevolverBlast();
                break;
            case 2:
                Blink();
                break;
        }
    }

   // Ability 1: Poison Throw 
    void ThrowPoison()
    {
        if (poisonPrefab != null && firePoint != null)
        {
            Instantiate(poisonPrefab, firePoint.position, transform.rotation);
            Debug.Log("Boss used Poison Throw!");
        }
    }

  // Ability 2: Revolver Blast 
    // Immediate Hitscan damage (Hit check)
    void RevolverBlast()
    {
        
        Debug.Log("Boss used Revolver Blast!");
        // Simple Raycast attack
        if (Physics.Raycast(firePoint.position, transform.forward, out RaycastHit hit, attackRange))
        {
            Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red, 1f);
            if (hit.collider.CompareTag("Player"))
            {
                CharacterStats pStats = hit.collider.GetComponent<CharacterStats>();
                if (pStats != null) pStats.TakeDamage(10); // Gun Damage
            }
        }
    }

    // Ability 3: Blink 
    // Teleport behind player or random spot
    void Blink()
    {
        Vector3 randomPos = Random.insideUnitSphere * 5f + _player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position);
            transform.LookAt(_player);
            Debug.Log("Boss Blinked!");
        }
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

    void ChaseBehavior()
    {

        _agent.speed = chaseSpeed;
        _agent.SetDestination(_player.position); // Update path to Player constantly
        AudioManager.AMInstance.setEpicState = true;

    }
}