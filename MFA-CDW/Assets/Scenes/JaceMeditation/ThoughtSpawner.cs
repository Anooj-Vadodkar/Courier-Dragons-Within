using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 3f;
    [SerializeField] private GameObject[] thoughtPrefabs;

    [SerializeField] private float spawnTimer = .5f;

    [SerializeField] private float spawnDistance = 2.0f;

    private int spawnIndex = 0;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            spawnTimer = spawnRate;

            Vector3 dirFromSpawner = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            dirFromSpawner.Normalize();

            GameObject obj = Instantiate(thoughtPrefabs[spawnIndex], transform.position + (dirFromSpawner * spawnDistance), Quaternion.identity);
            if(obj.GetComponent<HomingThought>() != null) {
                obj.GetComponent<HomingThought>().SetInitialDir(dirFromSpawner);
            }

            spawnIndex++;
            if (spawnIndex >= thoughtPrefabs.Length)
            {
                spawnIndex = 0;
            }
        }
    }
}
