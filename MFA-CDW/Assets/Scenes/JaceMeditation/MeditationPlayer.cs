using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MeditationPlayer : MonoBehaviour
{
    //parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] public Transform target;
    [SerializeField] private float breathInSpeed = 1f;
    [SerializeField] private float breathOutSpeed = 2f;
    [SerializeField] private float breathLeeway = .5f;
    [SerializeField] private float mentalDecayRate = .1f;
    [SerializeField] private float thoughtDecayRate = .1f;
    [SerializeField] private float breathEffectiveness = .1f; //how much does breathing help mental state
    [SerializeField] private AnimationCurve thoughtEffectiveness; //how much thoughts affect player at what mental state
    [SerializeField] private Color breathBaseColor = Color.blue;
    [SerializeField] private Color breathCompleteColor = Color.yellow;
    [SerializeField] private TextMeshProUGUI thoughtText;

    //states
    public float mentalState = .5f; //0-1, 1 being high
    private float breathProgress = 0f;
    private bool breathingIn = true;
    public bool canMove = true;
    private float breathTimer;
    private ThoughtBullet currentThought;
    private JaceSnareThought currentSnareThought;
    
    //refs
    private Background background;
    private Rigidbody2D rb;
    private BeliefTuner tuner;
    private Image breathMeter;

    [Header("Patrick Merge Variables")]
    [SerializeField]
    private GameObject cursorSprite;
    [SerializeField]
    private FieldOfView fov;
    [SerializeField]
    private Vector3 fovOffset;

    private void Start()
    {
        background = FindObjectOfType<Background>();
        rb = GetComponent<Rigidbody2D>();
        breathTimer = breathLeeway;
        tuner = GetComponentInChildren<BeliefTuner>();
        breathMeter = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (canMove)
        {
            TakeMoveInput();
        }
        if (currentThought)
        {
            MoveWithThought();
        }
        HandleBreathing();
        if (breathTimer <= 0)
        {
            DecayMental();
        }
    }

    private void FixedUpdate() {/* 
        Vector2 movementVec = InputManager.Instance.GetPlayerMovement();

        rb.AddForce(movementVec * moveSpeed, ForceMode2D.Force); */

        // Look Direction
        Vector2 mousePos = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        cursorSprite.transform.position = mousePos;

        Vector2 lookDirection = mousePos - new Vector2(transform.position.x, transform.position.y);

        fov.SetAimDirection(lookDirection.normalized);
        fov.SetOrigin(transform.position + fovOffset);
    }

    private void HandleBreathing()
    {
        if (Input.GetKey("space") || (Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5")))
        {
            if (breathProgress < 1)
            {
                breathProgress += breathInSpeed * Time.deltaTime;
                if(breathingIn) breathTimer = breathLeeway;
            }
            if(breathProgress >= 1 && breathingIn)
            {
                breathProgress = 1;
                breathingIn = false;
                breathTimer -= Time.deltaTime;
            }
            else if (!breathingIn)
            {
                breathTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (breathProgress > 0)
            {
                breathProgress -= breathOutSpeed * Time.deltaTime;
                if(!breathingIn) breathTimer = breathLeeway;
            }
            if(breathProgress <= 0 && !breathingIn)
            {
                breathProgress = 0;
                breathingIn = true;
                mentalState += breathEffectiveness;
                if (currentThought)
                {
                    ExitThought(currentThought);
                }
                if (currentSnareThought)
                {
                    ExitThought(currentSnareThought);
                }
                breathTimer -= Time.deltaTime;
            }
            else if (breathingIn)
            {
                breathTimer -= Time.deltaTime;
            }
        }

        breathMeter.fillAmount = breathProgress;
        if (breathProgress >= 1)
        {
            breathMeter.color = breathCompleteColor;
        }
        else breathMeter.color = breathBaseColor;
        if(background.enabled) background.UpdateColor(mentalState);
    }

    private void DecayMental()
    {
        mentalState -= mentalDecayRate * Time.deltaTime;
        if (currentThought || currentSnareThought) mentalState -= thoughtDecayRate * Time.deltaTime;
        if (mentalState <= 0)
        {
            mentalState = 0;
             StartCoroutine(Fail());
        }
        else if (mentalState >= 1)
        {
            mentalState = 1;
            //whatever else
        }
        if(background.enabled) background.UpdateColor(mentalState);
    }

    private IEnumerator Fail()
    {
        GameObject bg = background.gameObject;
        float colorProgress = 0f;
        Color startColor = bg.GetComponent<Image>().color;
        background.enabled = false;
        while (colorProgress < 1)
        {
            colorProgress += Time.deltaTime;
            bg.GetComponent<Image>().color = Color.Lerp(startColor, Color.red, colorProgress);
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void TakeMoveInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        transform.Translate(input * moveSpeed * Time.deltaTime, Space.World);
        FindObjectOfType<BackgroundScroller>().Scroll(input * moveSpeed * Time.deltaTime);
        if(input != Vector2.zero) UpdateRotation(input);
    }

    private void UpdateRotation(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0,0, newAngle);
    }

    public void EnterThought(ThoughtBullet thought)
    {
        currentThought = thought;
        thoughtText.text = thought.text;
    }
    
    public void EnterThought(JaceSnareThought thought)
    {
        currentSnareThought = thought;
        thoughtText.text = thought.text;
    }

    public void ExitThought(ThoughtBullet thought)
    {
        if (thought == currentThought)
        {
            currentThought = null;
            thoughtText.text = "";
        }
    }

    public void ExitThought(JaceSnareThought thought)
    {
        if (thought == currentSnareThought)
        {
            currentSnareThought = null;
            thoughtText.text = "";
            Destroy(thought.gameObject);
        }
    }

    private void MoveWithThought()
    {
        Vector2 translation = currentThought.currentDirection * currentThought.moveSpeed *
                              thoughtEffectiveness.Evaluate(mentalState) * Time.deltaTime;
        transform.position += (Vector3)translation;
        FindObjectOfType<BackgroundScroller>().Scroll(translation);
    }

    public void Lock() {
        canMove = false;
        rb.velocity = Vector2.zero;
    }

    public void Unlock() {
        canMove = true;
        rb.velocity = Vector2.zero;
    }
}
