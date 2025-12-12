using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponType type;
    public int damageAmount;
    public Color weaponColor;

    // Programmer Art: Auto-color the pickup so we know which is which
    void Start()
    {
        var rend = GetComponentInChildren<MeshRenderer>();
        if (rend != null)
        {
            rend.material.color = weaponColor;
        }
    }

    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0); // Spin effect
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombat combat = other.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                // Equip the new stats
                combat.EquipWeapon(type, damageAmount, weaponColor);

                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
}