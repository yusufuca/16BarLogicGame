using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 15f;
    public int impactDamage = 5;
    public int poisonDamage = 5; 
    public float poisonDuration = 5f; 
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats playerStats = other.GetComponent<CharacterStats>();

            if (playerStats != null)
            {
              
                playerStats.TakeDamage(impactDamage);

               
                playerStats.ApplyPoison(poisonDamage, poisonDuration);
            }

            DestroyProjectile();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
       
        Destroy(gameObject);
    }
}