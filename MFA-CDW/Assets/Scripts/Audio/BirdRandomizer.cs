using FMOD.Studio;
using SeagullPlayer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace FMODUnity
{
    public class BirdRandomizer : MonoBehaviour
    {
        public Transform Player;

        public GameObject objectToSpawn;

        public float minSpawnInterval = 1f;  // Minimum time between spawns
        public float maxSpawnInterval = 5f;  // Maximum time between spawns
        public float minSpawnRadius = 5f;
        public float maxSpawnRadius = 15f;

        private float nextSpawnTime;

        public List<GameObject> spawnList = new List<GameObject>();    
        void Start()
        {
            
            if (Player == null)
            {
                Debug.Log("Player transform not assigned");
                return;
            }
            else
            {
                ScheduleNextSpawn();
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(Time.time >= nextSpawnTime)
            {
                SpawnObject();
                ScheduleNextSpawn();
            }
        }

        void SpawnObject()
        {
            if (spawnList.Count > 5)
            {
                for (int i = 0; i < spawnList.Count; i++)
                {
                    Destroy(spawnList[i]);  
                }
                spawnList.Clear();
            }

            Vector3 playerPos = Player.position;

            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = 0;
            Vector3 spawnPos = playerPos + randomDirection * Random.Range(minSpawnRadius, maxSpawnRadius);

            if (objectToSpawn)
            {
                GameObject birdSound = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
                spawnList.Add(birdSound);
            }
        }
        void ScheduleNextSpawn()
        {
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnTime = Time.time + spawnInterval;
        }

        /*
        private void OnDrawGizmos()
        {
            if (Player == null)
            {
                return;
            }

            // Draw a sphere representing the spawn area
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Player.position, maxSpawnRadius);

            // Optionally draw a preview of the object to be spawned
            if (objectToSpawn != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Player.position + Vector3.up * maxSpawnRadius, 0.5f);
            }
        }*/
    }
}

