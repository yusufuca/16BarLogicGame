using UnityEngine;

public enum WeaponType { Default, Fire, LifeSteal }

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon Status")]
    public WeaponType currentWeapon = WeaponType.Default;
    public int damage = 30; // Default Grey Damage

    [Header("Visual References")]
    public MeshRenderer handSwordRenderer; // Drag your character's SWORD mesh here
    public GameObject fireParticlePrefab; // Drag a particle prefab here (optional)

    [Header("Settings")]
    public float attackRange = 1.5f;
    public float attackRate = 2.0f;
    public float impactDelay = 0.4f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private Animator _animator;
    private CharacterStats _myStats; // To heal myself
    private float _nextAttackTime = 0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _myStats = GetComponent<CharacterStats>();
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
        // Restart Animation
        _animator.Play("Attack", 0, 0f);

        // Schedule Damage
        Invoke("DealDamage", impactDelay);
    }

    void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                // 1. Apply Base Damage
                enemyStats.TakeDamage(damage);

                // 2. Apply Special Effects
                ApplyWeaponEffect(enemyStats, enemy.transform.position);
            }
        }
    }

    void ApplyWeaponEffect(CharacterStats enemy, Vector3 hitPos)
    {
        switch (currentWeapon)
        {
            case WeaponType.LifeSteal:
                // Green: Heal player for small amount (e.g. 5 HP)
                if (_myStats != null) _myStats.Heal(5);
                break;

            case WeaponType.Fire:
                // Red: Spawn visual fire (and maybe extra logic later)
                if (fireParticlePrefab != null)
                {
                    Instantiate(fireParticlePrefab, hitPos + Vector3.up, Quaternion.identity);
                }
                Debug.Log("Burned Enemy!");
                break;

            case WeaponType.Default:
                // Grey: Just raw power (Damage is already higher)
                break;
        }
    }

    // NEW: Method to switch weapons from the Pickup Script
    public void EquipWeapon(WeaponType type, int newDamage, Color newColor)
    {
        currentWeapon = type;
        damage = newDamage;

        // Change Visual Color
        if (handSwordRenderer != null)
        {
            handSwordRenderer.material.color = newColor;
            // Ensure Emission matches if using glowing materials
            handSwordRenderer.material.SetColor("_EmissionColor", newColor);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}