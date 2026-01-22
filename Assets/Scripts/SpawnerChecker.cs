using UnityEngine;

public class SpawnerActivator : MonoBehaviour
{
    public GameObject[] spawners;
    public float activationDistance = 20f;

    void Update()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] != null && !spawners[i].activeSelf)
            {
                if (Vector3.Distance(transform.position, spawners[i].transform.position) <= activationDistance)
                {
                    spawners[i].SetActive(true);
                }
            }
        }
    }
}