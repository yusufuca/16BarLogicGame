using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bossPrefab; // The Boss prefab to spawn
    public Transform spawnPoint;  // Where the Boss should appear

    [Header("Optional: Cinematics/Audio")]
    public bool destroyAfterTrigger = true; // Should this trigger disappear after use?

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Prevent double spawning
        if (hasTriggered) return;

        // Check if the object entering is the Player
        if (other.CompareTag("Player"))
        {
            SpawnBoss();
            AudioManager.AMInstance.setAnxietyState = true;
        }
    }

    void SpawnBoss()
    {
        hasTriggered = true;
        Debug.Log("Player entered the arena! Spawning Boss...");

        if (bossPrefab != null && spawnPoint != null)
        {
            // Spawn the Boss at the spawnPoint position and rotation
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

            // Optional: If you need to access the spawned boss immediately (e.g., to set UI)
            // BossAI bossScript = boss.GetComponent<BossAI>();
        }

        // Trigger music via AudioManager (if needed immediately)
        // AudioManager.AMInstance.setEpicState = true; 

        // Cleanup the trigger so it doesn't happen again
        if (destroyAfterTrigger)
        {
            Destroy(gameObject);
        }
    }

    // Visual aid to see the spawn point connection in Editor
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