using UnityEngine;

// (Enum and Class remain the same)
public enum WeaponType { None, Default, Fire, LifeSteal }

[System.Serializable]
public class WeaponData
{
    public string name;
    public WeaponType type;
    public int damage;
    public Color color;
    public bool hasWeapon;
}

public class PlayerCombat : MonoBehaviour
{
    [Header("Inventory")]
    public WeaponData[] inventory;
    public int activeSlotIndex = 0;

    [Header("References")]
    public InventoryUI inventoryUI; // DRAG THE UI SCRIPT HERE
    public MeshRenderer handSwordRenderer;

    // ... (Other variables: attackRange, etc. keep them same) ...
    public float attackRange = 1.5f;
    public float attackRate = 2.0f;
    public float impactDelay = 0.4f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject fireParticlePrefab;

    private Animator _animator;
    private CharacterStats _myStats;
    private float _nextAttackTime = 0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _myStats = GetComponent<CharacterStats>();

        // Init Inventory
        if (inventory == null || inventory.Length != 3)
        {
            inventory = new WeaponData[3];
            for (int i = 0; i < 3; i++) inventory[i] = new WeaponData();
        }

        // Setup Default Weapon in Slot 1
        inventory[0].name = "Grey Slayer";
        inventory[0].type = WeaponType.Default;
        inventory[0].damage = 30;
        inventory[0].color = Color.white;
        inventory[0].hasWeapon = true;

        UpdateWeaponVisuals();
    }

    void Update()
    {
        // Switch Inputs
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchSlot(2);

        // Attack Input
        if (Time.time >= _nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1") && inventory[activeSlotIndex].hasWeapon)
            {
                Attack();
                _nextAttackTime = Time.time + 1f / attackRate;
                AudioManager.AMInstance.PlaySwordSwingSFX();
            }
        }
    }

    void SwitchSlot(int index)
    {
        activeSlotIndex = index;
        UpdateWeaponVisuals();
    }

    void UpdateWeaponVisuals()
    {
        // 1. Update 3D Model
        if (inventory[activeSlotIndex].hasWeapon)
        {
            handSwordRenderer.enabled = true;
            handSwordRenderer.material.color = inventory[activeSlotIndex].color;
            handSwordRenderer.material.SetColor("_EmissionColor", inventory[activeSlotIndex].color);
        }
        else
        {
            handSwordRenderer.enabled = false;
        }

        // 2. Update 2D UI
        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI(inventory, activeSlotIndex);
        }

        if (_myStats != null)
        {
            if (inventory[activeSlotIndex].type == WeaponType.LifeSteal)
            {
                // GDD: Regen Sword enables passive regen logic
                _myStats.EnableRegen(true);
            }
            else
            {
                _myStats.EnableRegen(false);
            }
        }
    }

    // REVISED: Logic to find empty slot
    public void EquipWeapon(WeaponType newType, int newDamage, Color newColor)
    {
        int slotToFill = -1;

        // A. Look for first Empty Slot
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].hasWeapon)
            {
                slotToFill = i;
                break;
            }
        }

        // B. If full, overwrite the Active Slot (Gameplay choice)
        if (slotToFill == -1)
        {
            slotToFill = activeSlotIndex;
        }

        // C. Fill Data
        inventory[slotToFill].type = newType;
        inventory[slotToFill].damage = newDamage;
        inventory[slotToFill].color = newColor;
        inventory[slotToFill].hasWeapon = true;
        inventory[slotToFill].name = newType.ToString();

        // D. Refresh visuals (Only if we modified the active slot, OR strictly update UI)
        UpdateWeaponVisuals();

        Debug.Log("Added " + newType + " to Slot " + (slotToFill + 1));
    }

    // ... (Keep existing Attack(), DealDamage(), ApplyWeaponEffect() logic) ...
    void Attack()
    {
        // CancelInvoke removed as per previous request
        _animator.Play("Attack", 0, 0f);
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
                WeaponData currentWeapon = inventory[activeSlotIndex];

                // NEW: Calculate Final Damage with Level Multiplier
                float multiplier = (_myStats != null) ? _myStats.damageMultiplier : 1.0f;
                int finalDamage = Mathf.RoundToInt(currentWeapon.damage * multiplier);

                enemyStats.TakeDamage(finalDamage);
                ApplyWeaponEffect(currentWeapon.type, enemyStats, enemy.transform.position);
            }
        }
    }

    void ApplyWeaponEffect(WeaponType type, CharacterStats enemy, Vector3 hitPos)
    {
        // ... (Keep existing effect logic) ...
        if (type == WeaponType.LifeSteal && _myStats != null) _myStats.Heal(5);
        if (type == WeaponType.Fire && fireParticlePrefab != null) Instantiate(fireParticlePrefab, hitPos + Vector3.up, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}