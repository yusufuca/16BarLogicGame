using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    public bool isDead = false;

    // Animator reference to play "Death" animation
    private Animator _animator;

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        // Or GetComponentInChildren<Animator>() if using the skeleton architecture
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " damage. HP: " + currentHealth);

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

        // Trigger Death Animation
        if (_animator != null)
        {
            // You need to add a "Die" trigger to your Animator Controller later
            _animator.SetTrigger("Die");
        }

        // Disable Logic (Movement/AI)
        // We disable specific components so the body stays but stops acting
        var ai = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (ai != null) ai.enabled = false;

        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        var input = GetComponent<TPSMovement>();
        if (input != null) input.enabled = false;

        // Remove the collider so we can walk over the corpse
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}