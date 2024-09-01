using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RingBreathe : MonoBehaviour
{
    private Vector3 defaultScale;

    private float breatheTimer;
    private bool breathedIn;

    public bool BreathingIn => breatheTimer > 0.0f && breathedIn;
    public bool BreathingOut => breatheTimer > 0.0f && !breathedIn;


    [SerializeField]
    private float BREATHE_TIME = 0.5f;
    [SerializeField] private MediPlayerController player;

    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        breathedIn = false;
        defaultScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.0f);
        transform.localScale = Vector3.zero;
        inputManager = InputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (breatheTimer > 0.0f)
        {
            breatheTimer -= Time.deltaTime;
            if(breatheTimer < 0.0f)
            {
                breatheTimer = 0.0f;
            }

            if(breathedIn)
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, defaultScale, (BREATHE_TIME - breatheTimer)/BREATHE_TIME);
            }
            else
            {
                transform.localScale = Vector3.Lerp(defaultScale, Vector3.zero, (BREATHE_TIME - breatheTimer) / BREATHE_TIME);
            }
        }
    }

    public bool IsActive()
    {
        return breatheTimer > 0.0f && breathedIn;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(breatheTimer > 0.0f && breathedIn)
        {
            if (collision.CompareTag("Bullet") || collision.CompareTag("ObstacleBreakable"))
            {
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Belief"))
            {
                Destroy(collision.gameObject);
                collision.gameObject.GetComponent<BulletSpawner>().Change();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (breatheTimer > 0.0f && !breathedIn)
        {
            if (collision.CompareTag("Bullet"))
            {
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Belief"))
            {
                collision.gameObject.GetComponent<BulletSpawner>().Change();
            }
            else if (collision.CompareTag("FreeingBelief")) {
                player.FadeOut();
            }
        }
    }
    private void Breathe(bool breathingIn)
    {
        breathedIn = breathingIn;

        breatheTimer = BREATHE_TIME;
    }

    public void OnBreatheIn(InputValue value)
    {
        if (!breathedIn && breatheTimer <= 0.0f)
        {
            // Debug.Log("in");
            Breathe(true);
        }
    }

    public void OnBreatheOut(InputValue value)
    {
        if (breathedIn && breatheTimer <= 0.0f)
        {
            // Debug.Log("out");
            Breathe(false);
        }
    }
}
