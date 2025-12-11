using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int damage = 20;
    public float attackRange = 2.0f;
    public float attackRate = 1.0f; // Attacks per second

    [Header("References")]
    public Transform attackPoint; // Where the hit happens (create this!)
    public LayerMask enemyLayers; // Who can we hit?

    private Animator _animator;
    private float _nextAttackTime = 0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Rate Limiter
        if (Time.time >= _nextAttackTime)
        {
            // Input: Fire1 is usually Left Click or Ctrl
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. Play Animation
        _animator.SetTrigger("Attack");

        // 2. Detect Enemies (SphereCast)
        // We create a sphere at 'attackPoint' with size 'attackRange'
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 3. Deal Damage
        foreach (Collider enemy in hitEnemies)
        {
            // Try to find HP component
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
        }
    }

    // DEBUG: Draw the hit sphere in Editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}