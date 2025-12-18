using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Regen Logic")]
    public bool isRegenerating = false;
    public float regenRate = 5f; // HP per second
    private float _lastDamageTime;

    [Header("Leveling (Player Only)")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public float damageMultiplier = 1.0f; // Increases as we level up

    [Header("Loot & Rewards")]
    public int xpValue = 20; // How much XP this enemy gives when killed
    public GameObject itemToDrop;

    [Header("References")]
    public HealthBar healthBar;
    public GameObject damageTextPrefab;

    private Animator _animator;
    private bool isDead;

    void Update()
    {
        // 1. Check Combat Status (5 seconds since last hit)
        if (Time.time > _lastDamageTime + 5.0f && isRegenerating)
        {
            // 2. Regenerate over time
            if (currentHealth < maxHealth)
            {
                // Heal roughly 'regenRate' per second
                if (Time.frameCount % 60 == 0) // Optimization: run once per 60 frames approx
                {
                    Heal(Mathf.RoundToInt(regenRate));
                }
            }
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        AudioManager.AMInstance.PlayDamageImpactSFX();
        if (_animator != null) _animator.SetTrigger("Hit");
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (damageTextPrefab != null)
        {
            GameObject popup = Instantiate(damageTextPrefab, transform.position + Vector3.up * 2.0f, Quaternion.identity);
            popup.GetComponent<DamageText>().SetText(damage);
        }

        if (currentHealth <= 0) Die();

        _lastDamageTime = Time.time; // Reset Combat Timer
    }

  

    public void EnableRegen(bool state)
    {
        isRegenerating = state;
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetHealth(currentHealth);
    }

    // NEW: Gain XP Method
    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log("Gained " + amount + " XP. Total: " + currentXP);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f); // Harder to level up next time

        // Stat Boosts
        maxHealth += 20;
        currentHealth = maxHealth; // Full Heal on level up
        damageMultiplier += 0.2f; // 20% more damage

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        Debug.Log("LEVEL UP! Level: " + currentLevel + " | Damage Mult: " + damageMultiplier);

        // Optional: Play Level Up Effect/Sound here later
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (_animator != null) _animator.SetTrigger("Die");
        if (itemToDrop != null) Instantiate(itemToDrop, transform.position + Vector3.up, Quaternion.identity);

        // XP Logic (Keep existing code)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && this.gameObject.tag != "Player")
        {
            player.GetComponent<CharacterStats>().GainXP(xpValue);
        }

        // NEW: Game Loop Logic
        if (this.gameObject.CompareTag("Player"))
        {
            // Player Died -> Game Over
            GameManager.GMInstance.GameOver();
            AudioManager.AMInstance.isPlayerDeath = true;
        }
        else if (this.gameObject.CompareTag("Boss")) // Make sure Boss object has tag "Boss"
        {
            // Boss Died -> Win
            GameManager.GMInstance.Victory();
            AudioManager.AMInstance.isVictory = true;
        }

        DisableComponents();
    }

    void DisableComponents()
    {
        if (GetComponent<EnemyAI>()) GetComponent<EnemyAI>().enabled = false;
        if (GetComponent<TPSMovement>()) GetComponent<TPSMovement>().enabled = false;
        if (GetComponent<UnityEngine.AI.NavMeshAgent>()) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if (GetComponent<CharacterController>()) GetComponent<CharacterController>().enabled = false;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}