using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;

    // Optional: Float animation
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0); // Spin
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats stats = other.GetComponent<CharacterStats>();
            if (stats != null)
            {
                // We need to create this Heal method next!
                stats.Heal(healAmount);
                Destroy(gameObject); // Remove orb
            }
        }
    }
}