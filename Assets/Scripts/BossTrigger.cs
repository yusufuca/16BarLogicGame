using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab;
    public Transform spawnPoint;  

    [Header("Optional: Cinematics/Audio")]
    public bool destroyAfterTrigger = true;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
       
        if (hasTriggered) return;

       
        if (other.CompareTag("Player"))
        {
            SpawnBoss();
            AudioManager.AMInstance.setAnxietyState = true;
        }
    }

    void SpawnBoss()
    {
        hasTriggered = true;
    
        if (bossPrefab != null && spawnPoint != null)
        {
            
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);


        }
        if (destroyAfterTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, 1f);
            Gizmos.DrawLine(transform.position, spawnPoint.position);
        }
    }
}