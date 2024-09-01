using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitingBelief : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField]
    private float detectionRadius = 5.0f;
    [SerializeField]
    private float triggerTime = 4.0f;

    [Header("References")]
    [SerializeField]
    private GameObject snareObject;
    [SerializeField]
    private Transform player;

    private float proximityTimer = 0.0f;
    private bool triggered = false;

    private Snare snare;
    void Start()
    {
        if(player == null)
        {
            Debug.LogError("Player transform not set in " + gameObject.name + ", please fix thanks!");
        }
    }

    void Update()
    {
        if(triggered) { return; }
        if(Vector2.Distance(player.position, transform.position) <= detectionRadius)
        {
            proximityTimer += Time.deltaTime;
            Debug.Log(proximityTimer);

            // Player has been in range of the limiting thought for too long
            if(proximityTimer >= triggerTime)
            {
                Triggered(player.position);
            }
        }
        else if(proximityTimer > 0.0f)
        {
            proximityTimer = 0.0f;
        }
    }

    public void BreakSnare()
    {
        Destroy(gameObject);
    }

    // Player becomes ensared to the limiting thought
    private void Triggered(Vector3 position)
    {
        snare = Instantiate(snareObject, position, Quaternion.identity).GetComponent<Snare>();
        if(snare == null)
        {
            Debug.LogError("Snare object does not have Snare script");
            return;
        }
        snare.SetBelief(this);
        snare.SetPlayer(player.gameObject);
        triggered = true;
        Vector3[] positions = snare.GetPointPositions();
        if(positions.Length > 0)
        {
            Vector3 offset = positions[positions.Length / 2] * snare.gameObject.transform.localScale.x;
            offset.z = transform.position.z;
            snare.gameObject.transform.position -= offset;
        }
    }
}
