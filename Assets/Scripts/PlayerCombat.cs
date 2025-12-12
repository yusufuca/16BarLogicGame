using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int damage = 20;
    public float attackRange = 1.5f;
    public float attackRate = 2.0f;
    public float impactDelay = 0.4f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private Animator _animator;
    private float _nextAttackTime = 0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= _nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. CHANGE: Removed CancelInvoke("DealDamage");
        // Now, previous attacks will continue to count down and deal damage
        // even if the animation is visually interrupted.

        // 2. ANIMATION: Force Restart
        // We still restart the animation to give the "responsive" feel.
        _animator.Play("Attack", 0, 0f);

        // 3. LOGIC: Schedule the new damage hit
        // These Invokes will now "stack up" in the background.
        Invoke("DealDamage", impactDelay);
    }

    void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            CharacterStats stats = enemy.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}