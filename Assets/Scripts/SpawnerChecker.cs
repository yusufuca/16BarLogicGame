using UnityEngine;

public class SpawnerActivator : MonoBehaviour
{
    public GameObject[] spawners;
    public float activationDistance = 20f;
    public 
    void Update()
    {
        AudioManager.AMInstance.isSpawnerThere = false;
        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] != null)
            {
                if (Vector3.Distance(transform.position, spawners[i].transform.position) <= activationDistance)
                {
                    if (!spawners[i].activeSelf)
                    {
                       
                        spawners[i].SetActive(true);
                        
                        
                        
                    }

                    if (spawners[i].GetComponent<SimpleSpawner>()._spawnedCount < spawners[i].GetComponent<SimpleSpawner>().totalToSpawn)
                    {
                        AudioManager.AMInstance.isSpawnerThere = true;
                    }
                }
                
            }
        }
    }
}