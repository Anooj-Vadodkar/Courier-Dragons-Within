using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float spawnDelay;

    [SerializeField]
    private float timeBetweenSpawns;

    [SerializeField]
    private string positiveText;

    [SerializeField]
    private TextMeshProUGUI text;

    private float spawnTimer;
    private bool spawning = true;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawning)
        {
            if (spawnTimer > 0.0f)
            {
                spawnTimer -= Time.deltaTime;
            }
            else
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        GameObject g = Instantiate(bulletPrefab, transform.parent);
        g.transform.position = transform.position;
        spawnTimer = timeBetweenSpawns;
    }

    public void Change()
    {
        text.text = positiveText;
        spawning = false;
    }
}
