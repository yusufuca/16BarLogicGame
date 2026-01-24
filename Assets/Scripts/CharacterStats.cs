using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Status Effects")]
    public bool isPoisoned = false;
    private float poisonDurationTimer;
    private float poisonTickTimer;
    private int poisonDamagePerTick;

    [Header("Regen Logic")]
    public bool isRegenerating = false;
    public float regenRate = 5f; 
    private float _lastDamageTime;

    [Header("Leveling (Player Only)")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public float damageMultiplier = 1.0f; 

    [Header("Loot & Rewards")]
    public int xpValue = 20;
    public GameObject itemToDrop;

    [Header("References")]
    public HealthBar healthBar;
    public XpBar xpbar;
    public GameObject damageTextPrefab;

    private Animator _animator;
    private bool isDead;

    void Update()
    {
        
        if (Time.time > _lastDamageTime + 5.0f && isRegenerating)
        {
        
            if (currentHealth < maxHealth)
            {
               
                if (Time.frameCount % 60 == 0)
                {
                    Heal(Mathf.RoundToInt(regenRate));
                }
            }
        }
        if (isPoisoned)
        {
            
            poisonDurationTimer -= Time.deltaTime;

            
            poisonTickTimer -= Time.deltaTime;

            
            if (poisonTickTimer <= 0)
            {
                TakeDamage(poisonDamagePerTick); 
                poisonTickTimer = 1f;
               
            }

          
            if (poisonDurationTimer <= 0)
            {
                isPoisoned = false;
               
            }
        }
        }

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        if(xpbar != null) xpbar.SetMaxXP(xpToNextLevel,currentLevel);
    }

    public void ApplyPoison(int damagePerSecond, float duration)
    {
        if (isPoisoned)
        {
            
            poisonDurationTimer = duration;
        }
        else
        {
            isPoisoned = true;
            poisonDamagePerTick = damagePerSecond;
            poisonDurationTimer = duration;
            poisonTickTimer = 1f; 
        }
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

        _lastDamageTime = Time.time; 
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

    public void GainXP(int amount)
    {
        currentXP += amount;
        xpbar.SetXp(currentXP);
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
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f); 
        xpbar.SetMaxXP(xpToNextLevel, currentLevel);

       
        maxHealth += 20;
        currentHealth = maxHealth; 
        damageMultiplier += 0.2f; 

        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        Debug.Log("LEVEL UP! Level: " + currentLevel + " | Damage Mult: " + damageMultiplier);

        
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (_animator != null) _animator.SetTrigger("Die");
        if (itemToDrop != null) Instantiate(itemToDrop, transform.position + Vector3.up, Quaternion.identity);

        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && this.gameObject.tag != "Player")
        {
            player.GetComponent<CharacterStats>().GainXP(xpValue);
        }

      
        if (this.gameObject.CompareTag("Player"))
        {
           
            GameManager.GMInstance.GameOver();
            AudioManager.AMInstance.isPlayerDeath = true;
            Destroy(gameObject, 1f);
        }
        else if (this.gameObject.CompareTag("Boss")) 
        {
           
            GameManager.GMInstance.Victory();
            AudioManager.AMInstance.isVictory = true;
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject, 1f);
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