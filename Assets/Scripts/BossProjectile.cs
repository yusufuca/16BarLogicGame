using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 15;
    public float lifeTime = 3f;
    public GameObject impactEffect; // Drag a particle here if you have one

    void Start()
    {
        Destroy(gameObject, lifeTime); // Auto-destroy to prevent lag
    }

    void Update()
    {
        // Move forward constantly
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            DestroyProjectile();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            DestroyProjectile(); // Hit a wall/floor
        }
    }

    void DestroyProjectile()
    {
        if (impactEffect != null) Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}