using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject npcPrefab; // Drag the Prefab here
    public int totalToSpawn = 5; // Hard limit
    public float spawnInterval = 2.0f; // Seconds between spawns
    public float spawnRadius = 4.0f; // Area size

    // Internal State
    private float _timer;
    private int _spawnedCount;

    void Update()
    {
        // 1. Check Limit
        if (_spawnedCount >= totalToSpawn) return;

        // 2. Count Time
        _timer += Time.deltaTime;

        // 3. Trigger Spawn
        if (_timer >= spawnInterval)
        {
            Spawn();
            _timer = 0f; // Reset timer
        }
    }

    void Spawn()
    {
        // A. Calculate Position
        // Get a random X/Y point inside a 2D circle (Flat ground logic)
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;

        // Convert to 3D (X, Y, Z). We use the Spawner's current Y height.
        Vector3 spawnPos = new Vector3(
            transform.position.x + randomPoint.x,
            transform.position.y,
            transform.position.z + randomPoint.y
        );

        // B. Create Object
        // Instantiate(Object, Position, Rotation)
        Instantiate(npcPrefab, spawnPos, Quaternion.identity);

        // C. Increment
        _spawnedCount++;
    }

    // VISUALIZATION: Draws a yellow wireframe in the Scene view to show the area
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}