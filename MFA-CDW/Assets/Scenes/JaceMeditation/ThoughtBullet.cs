using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBullet : MonoBehaviour
{
    [SerializeField] public float moveSpeed = .5f;
    [SerializeField] [Range(0,1)] private float homingAmount = 0f;
    [SerializeField] public string text;
    [SerializeField] private float lifetime = 15f;
    
    //states
    public Vector2 currentDirection;
    private bool onPlayer = false;
    
    //refs
    private MeditationPlayer player;

    private void Start()
    {
        player = FindObjectOfType<MeditationPlayer>();
    }

    private void Update()
    {
        if (homingAmount > 0 && !onPlayer)
        {
            ChangeDirection();
        }
        transform.Translate(currentDirection.normalized * moveSpeed * Time.deltaTime);
        if (!onPlayer)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 00)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ChangeDirection()
    {
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection.normalized, directionToPlayer, homingAmount * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //check for player
        if (col.GetComponent<MeditationPlayer>())
        {
            player.EnterThought(this);
            onPlayer = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        //check for player
        if (col.GetComponent<MeditationPlayer>())
        {
            player.ExitThought(this);
            onPlayer = false;
        }
        
    }
}
