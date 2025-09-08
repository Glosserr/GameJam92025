using UnityEngine;

public class OrcSpawner : MonoBehaviour
{
    public int count;
    public float spawnRadius;
    public GameObject Orc;

    void Start()
    {
        int spawned = 0;
        while (spawned < count)
        {
            float randomX = Random.Range(-spawnRadius, spawnRadius);
            float randomZ = Random.Range(-spawnRadius, spawnRadius);
            Vector3 position = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            Instantiate(Orc, position, Quaternion.identity);
            spawned += 1;
        }
    }
}
