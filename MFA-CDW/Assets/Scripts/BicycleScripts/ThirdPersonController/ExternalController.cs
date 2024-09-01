using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class ExternalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private GameObject cam;
    [SerializeField]
    private GameObject wakingUpCam;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject dialogue;
    // Components
    [SerializeField] private Animator animator;
    //private Rigidbody rb; OLD
    private CharacterController controller;

    // Animation Hashes
    private int speedHash;
    private int inControlHash;

    [Header("Movement")]
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float moveAcceleration; //change to accel
    [SerializeField]
    private float maxVelocity;
    [SerializeField]
    private float maxSprintVelocity;
    [SerializeField] public bool walkingZone = false;
    [SerializeField] private float maxStrollVelocity;
    [SerializeField] private float maxPowerWalkVelocity;
    [SerializeField] bool standing = false;
    [SerializeField] private GameObject getUpPrompt;
    public bool spawnDialogue = true;
    //movement states
    private Vector3 velocity = Vector3.zero;

    [Header("Debug")]
    [SerializeField]private Transform convoNavPosition;
    [SerializeField] private Transform convoNavBikePosition;
    [SerializeField] private Transform convoNavTwoPosition;
    [SerializeField] private Transform convoNavTwoBikePosition;
    [SerializeField] private Transform cavePosition;
    [SerializeField] private Transform caveBikePosition;
    [SerializeField] private Transform walkTwoPosition;
    [SerializeField] private Transform walkTwoBikePosition;
    [SerializeField] private Transform walkFourPosition;
    [SerializeField] private Transform walkFourBikePosition;
    [SerializeField] private Transform vistaTwoPosition;
    [SerializeField] private Transform vistaTwoBikePosition;
    [SerializeField] private ConversationSpawner conversation;
    [SerializeField] private ChangeScript changeScriptDebug;

    [Header("AI")]
    [SerializeField] private ExternalTagController externalAI;

    [Header("Ground Check")]
    public float playerheight;
    public LayerMask whatIsGround;
    private bool grounded;
    [SerializeField] private float gravityAcceleration = 9.8f;

    [Header("Cutscenes")]
    [SerializeField]
    public PlayableDirector enterMedCutscene;
    [SerializeField]
    private PlayableDirector exitMedCutscene;
    [SerializeField]
    public CarouselController walking1Carousel;
    [SerializeField]
    public SpawnLoader spawnPoint;
    public bool paused = true;
    private string meditationSceneName;

    private AudioManager audioManager;

    private EventInstance convoMusic;

    // Player Inputs
    private InputManager inputManager;
    private CameraController cameraController;
    private Vector3 inputDir;

    // Debug
    private Vector3 MoveToConversation2 = new Vector3(919.95f, 56.43f, 454.57f);
    private void Start() {
        inputManager = InputManager.Instance;
        audioManager = AudioManager.Instance;
        cameraController = CameraController.Instance;

        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();

        speedHash = Animator.StringToHash("Speed");
        inControlHash = Animator.StringToHash("inControl");

        
      //  PlayerCursor.Instance.DisableCursor();

        //convoMusic = audioManager.CreateEventInstance(FMODEvents.Instance.convoMusic);

        // GetUp();
        // StartCoroutine(GetUp());
    }

    private void Update()
    {
        if (Input.GetKeyDown("="))
        {
            this.transform.position = convoNavPosition.position;
            player.transform.position = convoNavBikePosition.position;
            //changeScriptDebug.DebugSkipDialogue();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            this.transform.position = convoNavTwoPosition.position;
            player.transform.position = convoNavTwoBikePosition.position;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.transform.position = cavePosition.position;
            player.transform.position = caveBikePosition.position;
            //changeScriptDebug.DebugSkipDialogue();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.transform.position = walkTwoPosition.position;
            player.transform.position = walkTwoBikePosition.position;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.transform.position = walkFourPosition.position;
            player.transform.position = walkFourBikePosition.position;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            this.transform.position = vistaTwoPosition.position;
            player.transform.position = vistaTwoBikePosition.position;
        }
    }

    private void FixedUpdate()
    {
        if (!paused)
        {
            //rotate player object
            Vector3 viewDir = transform.position - new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            orientation.forward = viewDir.normalized;

            // rotate player object
            Vector2 movementInput = inputManager.GetPlayerMovement();
            inputDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;

            // Sets animator paramater
            animator.SetFloat(speedHash, (controller.velocity.magnitude / maxVelocity) * 2);

            if (inputDir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.fixedDeltaTime * rotationSpeed);
            }


            MovePlayer();
        } 
        /*if (inputManager.GetSprintInput() > 0.0f && !animator.GetBool(inControlHash)){
            Debug.Log("Getting up.");
            GetUp();
        }*/
    }
    /// <summary>
    /// Sets the camera for the external character active or disactive
    /// </summary>
    public void CameraActive(bool isActive) {
        cam.SetActive(isActive);
    }

    /// <summary>
    /// Moves the player.
    /// </summary>
    private void MovePlayer()
    {
        //apply friction
        if (groundDrag * Time.fixedDeltaTime > velocity.magnitude)
        {
            velocity = Vector3.zero;
        }
        else {velocity = velocity.normalized * (velocity.magnitude - groundDrag * Time.fixedDeltaTime);}
        
        //apply acceleration
        velocity += inputDir.normalized * moveAcceleration * Time.fixedDeltaTime;

        //clamp to max velocity
         if (inputManager.GetSprintInput() > 0.0f)
        { 
            if(walkingZone == true)
            {
                //velocity = Vector3.ClampMagnitude(velocity, maxPowerWalkVelocity);
            }
            else {
                velocity = Vector3.ClampMagnitude(velocity, maxSprintVelocity); 
            }
            
        }
        else
        {
            if (walkingZone == true)
            {
                
                //velocity = Vector3.ClampMagnitude(velocity, maxStrollVelocity);
            }
            velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        } 

        CheckGrounded();
        
        controller.Move(velocity * Time.deltaTime); 
    }

    public void StartCarousel()
    {
        if(spawnPoint.GetSpawnIndex() == 1 && paused)
        {
            walking1Carousel.PlayCarousel();
            //dialogue.SetActive(true);
        }
            
    }

    public void GetUp(){
        Debug.Log("Hit Get Up");
        AudioManager._instance.PlayEvent(FMODEvents._instance.getUp);
        CameraActive(true);
        getUpPrompt.SetActive(false);
        standing = true;
        //cameraController.GetCurrentCam().SetActive(false);
        wakingUpCam.SetActive(false);
        paused = false;
        StartWalkAnimation();
        // StartCoroutine(GettingUp());
        animator.SetBool(inControlHash, true);
    }

    public void SitDownSound() {
        Debug.Log("Hit Sit Down");
        AudioManager._instance.PlayEvent(FMODEvents._instance.sitDown);
    }

/*     public IEnumerator GettingUp() {
        animator.SetBool(inControlHash, true);
        yield return new WaitForSeconds(0.0f);
        SetPaused(false);
    } */

    public void EnterMeditation() {
        Debug.Log("Hit Enter Meditation");
        audioManager.StopMusic();
        audioManager.StopEvent(FMODEvents.Instance.natureAmbient);
        GameManager.Instance.SceneTranstition(meditationSceneName);
    }

    public void PlayEnterMedCutscene(string sceneName) {
        SetPaused(true);
        meditationSceneName = sceneName;
        animator.SetBool(inControlHash, false);
        enterMedCutscene.Play();
        // Invoke("EnterMeditation", 6.5f);
    }

    public bool GetStanding()
    {
        return standing;
    }

    public void PlayExitMedCutscene() {
        // if(ambientSFX) audioManager.PlaySFXLoop(ambientSFX.clip);
        // if(ambientMusic) audioManager.PlayMusic(ambientMusic.clip);
        exitMedCutscene.Play();
    }

    public void SetPaused(bool value)
    {
        /* if(value) {
            convoMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        } else {
            convoMusic.start();
        } */
        paused = value;
    }
    
    public void StopWalkAnimation()
    {
        animator.gameObject.GetComponent<Animator>().enabled = false;
    }
    
    public void StartWalkAnimation()
    {
        animator.gameObject.GetComponent<Animator>().enabled = true;
    }

    /// <summary>
    /// Checks if the player is grounded | Shouldn't be needed using character controller but left here just in case
    /// </summary>
    private void CheckGrounded() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.2f, whatIsGround);

        if (!grounded)
            velocity -= new Vector3(0, gravityAcceleration * Time.deltaTime, 0);
        else
            velocity = new Vector3(velocity.x, 0, velocity.z);
    }

    public void SetAnimatorInControl(bool val)
    {
        animator.SetBool(Animator.StringToHash("inControl"), val);
    }
}
