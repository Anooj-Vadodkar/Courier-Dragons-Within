//using Microsoft.Unity.VisualStudio.Editor;
// using UnityEngine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using FMOD.Studio;
using Unity.VisualScripting;
using TMPro;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
// using UnityEditor.ShaderGraph.Internal;
using System;

public class MediMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float regularMaxSpeed;
    [SerializeField]
    private float confrontingMaxSpeed;
    private float speed = 0.0f;
    [SerializeField]
    private float maxSpeed;
    private float previousMaxSpeed;
    private Coroutine promptCoroutine;

    [Header("Acceleration")]
    [SerializeField]
    private AnimationCurve accelerationCurve;
    [SerializeField]
    private float accelerationMultiplier;

    [Header("Deceleration")]
    [SerializeField]
    private AnimationCurve decelerationCurve;
    [SerializeField]
    private float decelerationMultiplier;
    [SerializeField]
    private float minDistToCursor = 0.1f;
    [SerializeField]
    private float maxDistToCursor = 5.0f;

    // Directions
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 lastDirection = Vector2.zero;
    private Vector2 movementVector = Vector2.zero;
    private Vector2 lastLookDirection = Vector2.zero;

    // Singletons
    private InputManager inputManager;

    public bool paused = false;
    private bool confronting = false;
    private Thought currentThought;

    private EventInstance mediMusic;

    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private GameObject cursorObj;
    [SerializeField]
    private GameObject breathePrompt;
    public Animator breathPromptAnim;
    public Animator triggerAnim;
    [SerializeField]
    private SpriteRenderer breathIcon;
    private Vector3 startingBreathScale;
    [SerializeField]
    private AnimationCurve iconSpeed;
    private float breath;
    private bool canBreath = true;
    private bool breathedIn = false;

    [SerializeField]
    private float breathTime = 1.5f;

    [SerializeField]
    private GameObject breathCircle;
    [SerializeField]
    private float maxBreathSize;

    [SerializeField] private ScreenShakeScript screenShake;
    [SerializeField] private float colorChangeSpeed = 1;

    private void Start() {
        inputManager = InputManager.Instance;

        SetPaused(true);
        maxSpeed = regularMaxSpeed;
        previousMaxSpeed = maxSpeed; // Initialize previous max speed

        startingBreathScale = breathIcon.transform.localScale;
        breathIcon.gameObject.transform.localScale = Vector3.zero;
    }
    private void Update()
    {
        if (previousMaxSpeed != maxSpeed)  // !!!!!  This checks the players maxSpeed to display a prompt and should be moved to SlowPlayer to improve performance  !!!!!!!
        {
            previousMaxSpeed = maxSpeed;

            if (maxSpeed <= 0f && promptCoroutine == null  && paused == false)
            {
                promptCoroutine = StartCoroutine(ShowPromptCoroutine());
            }
            else if (maxSpeed >= 4.5f)
            {
                if (promptCoroutine != null)
                {
                    StopCoroutine(promptCoroutine);
                    promptCoroutine = null;
                }
                breathePrompt.SetActive(false);
                breathPromptAnim.SetBool("ShowBreathPrompt", false);
            }
        }
    }

    private void FixedUpdate() {
        if(!paused) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);

            // Getting the input
            if(inputManager.isUsingController) {
                movementVector = inputManager.GetPlayerMovement();
            } else {
                // Get input based off of mouse position to player
                if(inputManager.GetLeftClick()) {
                    if((mousePos - transform.position).magnitude > minDistToCursor) {
                        movementVector = (mousePos - transform.position).normalized;
                    } else {
                        // stop the player
                        movementVector = Vector2.zero;
                        speed = 0;
                    }
                } else {
                    movementVector = Vector2.zero;
                }
            }

            // Debug.Log(movementVector);

            if(movementVector != Vector2.zero) {
                float inputAmt = movementVector.magnitude;

                lastDirection = movementVector.normalized;

                // Lerping the look direction
                currentDirection = Vector2.Lerp(currentDirection, lastDirection, 0.2f);
                    
                // Acceleration
                if(speed < maxSpeed) {
                    if(maxSpeed != 0) {
                        speed += accelerationCurve.Evaluate(speed / Math.Abs(maxSpeed)) * accelerationMultiplier * inputAmt;
                    } else {
                        speed += accelerationCurve.Evaluate(speed / float.MinValue) * accelerationMultiplier * inputAmt;
                        speed = 0;
                    }
                    if(speed > maxSpeed) {
                        speed = maxSpeed;
                    }
                } else if(speed > maxSpeed) {
                    if(maxSpeed != 0) {
                        speed -= decelerationCurve.Evaluate(speed / Math.Abs(maxSpeed)) * decelerationMultiplier;
                    } else {
                        speed -= decelerationCurve.Evaluate(speed / -float.MinValue) * decelerationMultiplier;
                        speed = 0;
                    }
                    if(speed < 0.0f) {
                        speed = 0;
                    }
                }
            } else {
                // Deceleration
                if(speed > 0.0f) {
                    if(maxSpeed != 0) {
                        speed -= decelerationCurve.Evaluate(speed / Math.Abs(maxSpeed)) * decelerationMultiplier;
                    } else {
                        speed -= decelerationCurve.Evaluate(speed / -float.MinValue) * decelerationMultiplier;
                        speed = 0;
                    }
                    if(speed < 0.0f) {
                        speed = 0.0f;
                    }
                }
            }

            Debug.Log("Current speed: " + speed);

            // Moving
            transform.Translate(currentDirection * speed * Time.deltaTime);

            Vector2 dir = (transform.position - mousePos).normalized;

            cursorObj.transform.LookAt(mousePos);

            Vector2 lookDirection = (Vector2)mousePos - new Vector2(transform.position.x, transform.position.y);

            // Breath in time is 1.5 seconds

            // Check if holding breath
            if(InputManager.Instance.GetBreathInput() == 1) {
                if(canBreath) {
                    // if haven't started beathing in then start playing rbeath sound.
                    if(breath < breathTime) {
                        breath += Time.deltaTime;
                        if(breath >= breathTime){
                            // breathIcon.color = Color.blue;
                            StopCoroutine("ChangeBreathIconColor");
                            StartCoroutine(ChangeBreathIconColor(Color.cyan));
                            breath = breathTime;

                            // fully breathed in, wait for release
                            breathedIn = true;
                        }
                    }
                } else {
                    if(breath > 0) {
                        breath -= Time.deltaTime;
                        if(breath <= 0) {
                            breath = 0;
                        }
                    }
                }
            } else {
                // on release . . .
                // Stop breath sound.

                // if the player has fully breathed in ...
                if(breathedIn) {
                    breathedIn = false;
                    StopCoroutine("ChangeBreathIconColor");
                    StartCoroutine(ChangeBreathIconColor(Color.white));
                    // breathIcon.color = Color.white;

                    // reset speed
                    if(confronting) {
                        maxSpeed = confrontingMaxSpeed;

                        // Push thoughts away
                        StartCoroutine(Breath());
                    }
                }

                if(breath > 0) {
                    breath -= Time.deltaTime;
                    if(breath <= 0) {
                        breath = 0;
                    }
                }
            }

            // Increase scale of breath icon
            breathIcon.transform.localScale = Vector3.Lerp(Vector3.zero, startingBreathScale, breath);
            // Icon rotation
            icon.transform.Rotate(Vector3.forward * iconSpeed.Evaluate(breath));
        }
    }

    public void SetPaused(bool isPaused) {
        paused = isPaused;
        if(!isPaused) {
            PlayerCursor.Instance.BaseRetical();
            PlayerCursor.Instance.gameObject.SetActive(true);
        } else {
            // PlayerCursor.Instance.DisableCursor();
            PlayerCursor.Instance.gameObject.SetActive(false);
        }
    }

    public void BeginConfronting(Vector3 position, Thought thought) {
        // mediMusic.setParameterByName("PAR_MEDI_MX_SWITCHER", 1);
        SetPaused(true);
        maxSpeed = confrontingMaxSpeed;
        confronting = true;
        currentThought = thought;
        StartCoroutine(MoveToConfrontingPoint(position));
    }

    private IEnumerator MoveToConfrontingPoint(Vector3 position) {
        float lerpAmount = 0;
        transform.position = Vector3.Lerp(transform.position, position, lerpAmount);
        yield return new WaitForEndOfFrame();
        while(lerpAmount < 1) {
            lerpAmount += 0.3f * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, position, lerpAmount);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1.0f);
        SetPaused(false);
    }

/*     public void SereneState() {
        // mediMusic.setParameterByName("PAR_MEDI_MX_SWITCHER", 2);
    } */

    public IEnumerator Breath() {
        canBreath = false;
        float scale, alpha = 1;
        float timer =- 0;
        float lerpAmt;
        Color newColor = breathCircle.GetComponent<SpriteRenderer>().color; 
        while(timer < breathTime) {
            timer += Time.deltaTime;
            lerpAmt = timer / breathTime;
            alpha = Mathf.Lerp(1, 0, lerpAmt);
            scale = Mathf.Lerp(1, 5, lerpAmt);
            breathCircle.transform.localScale = new Vector3(scale, scale, scale);
            breathCircle.GetComponent<SpriteRenderer>().color = new Color(newColor.r, newColor.g, newColor.b, alpha);
            yield return null;
        }
        breathCircle.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
        breathCircle.GetComponent<SpriteRenderer>().color = new Color(newColor.r, newColor.g, newColor.b, 0);
        canBreath = true;
    }

    // Function to slow down the player
    public void SlowPlayer(float slowAmt)
    {
        // Debug.Log("Hit slow player");
        screenShake.TriggerShake(0.1f);
        maxSpeed -= slowAmt;
    }

    // Coroutine to show the prompt after a delay
    IEnumerator ShowPromptCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        breathePrompt.SetActive(true);
        breathPromptAnim.SetBool("ShowBreathPrompt", true);
        StartTriggerAnimation();
    }

    IEnumerator StartTriggerAnimation()
    {
        yield return new WaitForSeconds(4);

        triggerAnim.SetBool("TriggerAnim", true);

    }

    private IEnumerator ChangeBreathIconColor(Color newColor) {
        Color color = breathIcon.color;
        float lerpAmt = 0;
        while(lerpAmt < 1) {
            lerpAmt += Time.deltaTime * colorChangeSpeed;
            if(lerpAmt > 1) {
                lerpAmt = 1;
            }
            color = Color.Lerp(color, newColor, lerpAmt);
            breathIcon.color = color;
            yield return new WaitForSeconds(0.0f);
        }
    }
}
