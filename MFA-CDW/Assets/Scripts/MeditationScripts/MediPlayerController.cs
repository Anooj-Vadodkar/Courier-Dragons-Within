using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class MediPlayerController : MonoBehaviour
{
    [SerializeField]
    private FieldOfView fov;

    [SerializeField]
    private float maxSpeed = 10.0f;

    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private GameObject cursorSprite;

    [SerializeField]
    private PlayableDirector director;

    [SerializeField]
    private Vector3 fovOffset;

    private bool paused;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Movement
        if(!paused) {
            Vector2 movementVec = InputManager.Instance.GetPlayerMovement();

            rb.AddForce(movementVec * maxSpeed, ForceMode2D.Force);
        }

        // Look Direction
        Vector2 mousePos = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        cursorSprite.transform.position = mousePos;

        Vector2 lookDirection = mousePos - new Vector2(transform.position.x, transform.position.y);
        sprite.transform.up = lookDirection.normalized;

        // Set fov aim and pos
        fov.SetAimDirection(lookDirection.normalized);
        fov.SetOrigin(transform.position + fovOffset);
    }

    public void Lock() {
        paused = true;
        rb.velocity = Vector2.zero;
    }

    public void Unlock() {
        paused = false;
        rb.velocity = Vector2.zero;
    }

    public void FadeOut() {
        director.Play();
    }
}
