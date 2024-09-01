using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class ConfBulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletFreq;

    [SerializeField] private BossPatternManager bpm;
    [SerializeField] private float angleOffset;
    [SerializeField] private float spawnDistFromCenter = 2;

    private int waveIndex;
    private float timer = 0;

    public bool isShooting = true;

    private int sections, streamCount, bulletCount;

    private GameObject bulletInstance;

    private void Start() {
        if(bpm == null) {
            bpm = GetComponent<BossPatternManager>();
        }
    }

    private void Update() {
        if(isShooting && timer >= bulletFreq) {
            timer = 0;
            SpawnWave();
        }
        timer += Time.deltaTime;
    }

    public List<GameObject> SpawnWave() {
        List<GameObject> wave = new List<GameObject>();
        sections = bpm.data.Length;
        if(sections > 0) {
            streamCount = bpm.data[0].sc;
            bulletCount = bpm.data[0].bc;
            float sectionSize = (360.0f / sections);

            float streamDist = 0;
            if(streamCount > 0) {
                streamDist = Mathf.Floor(sectionSize / streamCount);
                // Loop through sections
                for(int section = 0; section < sections; section++) {
                // float sectionStartAngle = i * sectionSize;
                // float angle = sectionStartAngle;
                    float angle = section * sectionSize + angleOffset;
                    
                    for(int stream = 0; stream < streamCount; stream++) {
                        if(bpm.data[section].rows[stream].row[waveIndex]) {
                            bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
                            bulletInstance.transform.position = bulletInstance.transform.position + bulletInstance.transform.up * spawnDistFromCenter;
                            if(bulletInstance.GetComponent<Bullet>()) {
                                bulletInstance.GetComponent<Bullet>().AssignThought(GetComponent<Thought>());
                            }
                            if(bulletInstance.GetComponent<HomingThought>()) {
                                bulletInstance.GetComponent<HomingThought>().AssignOrigin(transform.position);
                            }
                            wave.Add(bulletInstance);
                        }
                        angle += streamDist;
                    }
                }
            }
        }
        waveIndex++;
        if(waveIndex >= bulletCount) {
            waveIndex = 0;
        }
        return wave;
    }

    public void SetSpawnerActive(bool active) {
        Debug.Log("Hit SetSpawnerActive");
        isShooting = active;
    }
}
