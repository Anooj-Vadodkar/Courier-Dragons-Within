using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;
using Unity.VisualScripting;

public class ConfrontAtEnd : MonoBehaviour
{

    public Animator promptAnim;
    public Animator triggerAnim;
    public Animator ringAnim;

    //parameters
    [SerializeField] private float endSize;
    [SerializeField] private float breatheInTime = 2f;
    [SerializeField] private float breatheOutTime = .8f;
    [SerializeField] public UnityEvent onBreathed;

    // Breath prompts
    [SerializeField] private GameObject breathPrompt;
    [SerializeField] private EventReference eventRef;
    [SerializeField] private GameObject collisionGate;
    [SerializeField] private NextLabyrinthSectionActivate transitionSection;

    [SerializeField] private GameObject LabyrinthOverall;
    [SerializeField] private GameObject PartialLabyrinth;


    //states
    private float breathValue = 0f;
    private bool breathedIn = false;
    private Vector2 startPos;
    private float startSize;
    private float startAlpha;

    //refs
    private Transform player;
    private Thought thought;
    private SpriteRenderer sprite;
    private AudioManager audioManager;

    private EventInstance breathInstance;
    private bool isBreathPlaying = false;
    private bool isBreathingOut = false;

    private bool firstTimeEntered = true;
    [SerializeField] private UnityEvent enteredEvent;

    [SerializeField]
    private EndMeditation endMeditation;

    private void Awake()
    {
        startPos = transform.position;
        startSize = transform.localScale.x;
        player = FindObjectOfType<MediMovement>().transform;
        startAlpha = GetComponent<SpriteRenderer>().color.a;
        thought = FindObjectOfType<Thought>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        breathInstance = audioManager.CreateEventInstance(FMODEvents.Instance.breathIn);
    }

    public void FadeIn(float time)
    {
        Color targetColor = sprite.color;
        targetColor.a = 0;
        sprite.color = targetColor;
        StartCoroutine(FadeInRoutine(time));
    }

    private IEnumerator FadeInRoutine(float time)
    {
        float progress = 0;
        while (progress < 1)
        {
            // Debug.Log("progress: " + progress + "\nAlpha: " + sprite.color.a);
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime / time;
            progress = Mathf.Clamp01(progress);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b,
                Mathf.Lerp(0, startAlpha, progress));
        }
    }

    // Start is called before the first frame update
    private void OnTriggerStay2D(Collider2D other)
    {
        //player is inside zone
        if (other.CompareTag("Player"))
        {
            if (!isBreathingOut)
            {
                if (InputManager.Instance.GetBreathInput() == 1f)
                {
                    if (!isBreathPlaying)
                    {
                        isBreathPlaying = true;
                        breathInstance.start();
                    }
                    if (!isBreathingOut)
                    {
                        breathValue += Time.fixedDeltaTime / breatheInTime;
                    }
                }
            }

            if (InputManager.Instance.GetBreathInput() == 0 || isBreathingOut)
            {
                if (isBreathPlaying)
                {
                    isBreathPlaying = false;
                    breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                breathValue -= Time.fixedDeltaTime / breatheOutTime;

                if (breathedIn && !isBreathingOut)
                {
                    isBreathingOut = true;
                    audioManager.PlayEvent(FMODEvents.Instance.breathOut, Camera.main.transform.position);
                }
            }


            breathValue = Mathf.Clamp01(breathValue);
            if (breathValue == 1f)
            {
                breathedIn = true;
                if (breathPrompt)
                {
                    breathPrompt.SetActive(false);
                }
            }

            if (!breathedIn)
            {
                BreatheInVisuals(breathValue, (InputManager.Instance.GetBreathInput() == 1f));
            }
            else
            {
                endMeditation.Exit();
                BreatheOutVisuals(1 - breathValue);
                if (breathValue == 0f)
                {
                    onBreathed.Invoke();
                }
            }
        }
    }

    private void BreatheInVisuals(float value, bool pressed)
    {
        float newScale = Mathf.Lerp(startSize, .1f, value);
        transform.localScale = new Vector3(newScale, newScale, 1);

        float maxMoveDistance = ((Vector2)player.position - startPos).magnitude / breatheInTime * Time.deltaTime;
        if (pressed)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, maxMoveDistance);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, maxMoveDistance);
        }
    }

    private void BreatheOutVisuals(float value)
    {
        transform.position = player.position;
        float newScale = Mathf.Lerp(.1f, endSize, value);
        transform.localScale = new Vector3(newScale, newScale, 1);
        Color newColor = sprite.color;
        newColor.a = Mathf.Lerp(startAlpha, 0, value);
        sprite.color = newColor;
    }
}
