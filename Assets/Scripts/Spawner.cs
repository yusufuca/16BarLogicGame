using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject npcPrefab;
    public int totalToSpawn = 5; 
    public float spawnInterval = 2.0f; 
    public float spawnRadius = 4.0f; 
   
 
    private float _timer;
    public int _spawnedCount;

    void Update()
    {

        if (_spawnedCount >= totalToSpawn) return;


        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            Spawn();
            _timer = 0f; 
        }
    }

    void Spawn()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(
            transform.position.x + randomPoint.x,
            transform.position.y,
            transform.position.z + randomPoint.y
        );

     
        Instantiate(npcPrefab, spawnPos, Quaternion.identity);
        _spawnedCount++;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}