using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScrolling : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private float furthestXVal = 280f;
    [SerializeField] private Transform spawnPoint;

    private void Start() {
        RandomizeYPos();
    }

    private void Update() {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
        if(transform.position.x > furthestXVal) {
            transform.position = spawnPoint.position;
            RandomizeYPos();
        }
    }

    private void RandomizeYPos() {
        transform.position = new Vector3(transform.position.x, Random.Range(-30, 30), transform.position.z);
    }
}
