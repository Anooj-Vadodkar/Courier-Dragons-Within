using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;
public class ExternalTagController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;

    // Components
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float distanceFromChase;
    //private Rigidbody rb; OLD
    //private NavMeshAgent controller;

    // Animation Hashes
    private int speedHash;
    private int inControlHash;

    public NavMeshAgent tagNavMesh;

    [Header("Movement")]
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float moveAcceleration; //change to accel
    [SerializeField]
    private float maxVelocity=3.5f;
    [SerializeField]
    private float maxSprintVelocity;

    [SerializeField]
    private bool startsSitting = true;
    private bool followPlayer = false;
    [SerializeField]
    private bool IndependentWalking = false;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Vector3 destination;

    [SerializeField]
    private Transform walking2Dest;

    private bool paused = false;

    private AudioManager audioManager;

    private EventInstance convoMusic;

    private void Start()
    {
        tagNavMesh = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        audioManager = AudioManager.Instance;
        speedHash = Animator.StringToHash("Speed");
        inControlHash = Animator.StringToHash("inControl");
        if (destination != null && IndependentWalking)
            SetDestination(destination) ;
        if (!startsSitting)
            GetUp();
        //GetUp();
    }

    public void ResetDestination()
    {
        tagNavMesh.ResetPath();
    }

    private void FixedUpdate()
    {
        animator.SetFloat(speedHash, (tagNavMesh.velocity.magnitude));
        if (IndependentWalking && player != null && destination != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < distanceFromChase)
            {
                tagNavMesh.SetDestination(destination);
            }
            else
            {
                if(tagNavMesh.isActiveAndEnabled) {
                    tagNavMesh.ResetPath();
                }
            }
        }
        
    }
    public void SetFollowPlayer(bool following)
    {
        followPlayer = following; 
    }
    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
    }

    public void GetUp()
    {
        StartCoroutine(GettingUp());
    }

    public IEnumerator GettingUp()
    {
        animator.SetBool(inControlHash, true);
        yield return new WaitForSeconds(0.0f);
        SetPaused(false);
    }

    public void EnterMeditation(string sceneName)
    {
        GameManager.Instance.SceneTranstition(sceneName);
    }

    public void PlayWaveAnimation()
    {
        StartCoroutine("PlayWave");
    }

    public void DebugWalking3Teleport()
    {
        this.transform.position = walking2Dest.position;
    }

    IEnumerator PlayWave()
    {
        yield return new WaitForSeconds(4.0f);
        animator.Play("TagWave");
    }

    public void SetPaused(bool value)
    {
        paused = value;
    }

    public void SetWalking(bool walking)
    {
        IndependentWalking = walking;
    }

}
