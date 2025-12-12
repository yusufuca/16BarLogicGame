using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Loot Settings")]
    public GameObject itemToDrop; // Drag the HP_Orb Prefab here (Only for Enemies)

    [Header("References")]
    public HealthBar healthBar;
    public GameObject damageTextPrefab; // DRAG YOUR POPUP PREFAB HERE

    private Animator _animator;
    private bool _isDead;

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        currentHealth -= damage;

        if (damageTextPrefab != null)
        {
            // Spawn slightly above the head (Y + 2.0)
            GameObject popup = Instantiate(damageTextPrefab, transform.position + Vector3.up * 2.0f, Quaternion.identity);

            // Set the number
            popup.GetComponent<DamageText>().SetText(damage);
        }

        // Hit Reaction Logic
        if (_animator != null) _animator.SetTrigger("Hit");
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.enabled) agent.SetDestination(transform.position);

        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0) Die();
    }

    // NEW: Healing Method
    public void Heal(int amount)
    {
        if (_isDead) return;

        currentHealth += amount;

        // Clamp: Ensure we don't go over Max Health
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null) healthBar.SetHealth(currentHealth);
        Debug.Log(transform.name + " Healed " + amount);
    }

    void Die()
    {
        _isDead = true;
        if (_animator != null) _animator.SetTrigger("Die");

        // NEW: Drop Logic
        if (itemToDrop != null)
        {
            // Spawn the orb slightly above the ground (Y + 1.0)
            Instantiate(itemToDrop, transform.position + Vector3.up, Quaternion.identity);
        }

        // Disable Components
        if (GetComponent<UnityEngine.AI.NavMeshAgent>()) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if (GetComponent<CharacterController>()) GetComponent<CharacterController>().enabled = false;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (GetComponent<EnemyAI>()) GetComponent<EnemyAI>().enabled = false;

        this.enabled = false;
    }
}