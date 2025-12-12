using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    public bool isDead = false;

    [Header("UI References")]
    public HealthBar healthBar; // Drag the Canvas/Slider object here (Only for Player)

    // Animator reference to play "Death" animation
    private Animator _animator;

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        // Or GetComponentInChildren<Animator>() if using the skeleton architecture

        // Initialize UI if assigned
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " damage. HP: " + currentHealth);

        // Update UI
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // Optional: Play Hit Reaction animation here

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log(transform.name + " Died.");

        // FIX A: Ensure parameter exists before calling (or just add it in Animator)
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }

        // FIX B: Disable the BRAIN (Script) first!
        // This stops the Update() loop from trying to move the agent.
        var enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        // Disable the Player Input if this is the player
        var playerInput = GetComponent<TPSMovement>();
        if (playerInput != null) playerInput.enabled = false;

        // Disable the Body (Physics/NavMesh)
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        // Finally, disable this Stats script
        this.enabled = false;
    }
}