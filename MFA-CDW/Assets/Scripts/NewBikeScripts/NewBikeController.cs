using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Collections.Specialized;
using System;
using Cinemachine;
using System.Data.Common;
using UnityEngine.Animations.Rigging;

public class NewBikeController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool paused = false;
    [SerializeField] private bool canMount = false;
    [SerializeField] private GameObject inputUIPanel;
    [SerializeField] private CinemachineVirtualCamera shoulderCam;
    [SerializeField] private GameObject pedalUIPrompt;
    [SerializeField] private int promptDisablePedalAmt = 2;
    [SerializeField] private float timeForPedalPrompt = 7;
    private float pedalTimer;
    private int pedalCount;

    [Header("Movement")]
    [SerializeField] private PathCreator path;
    [SerializeField] private float pedalForce = 3;
    [SerializeField] private float minTimeBetweenPedal = 0.1f;
    [SerializeField] private float maxTimeBetweenPedal = 0.5f;
    private float minVelocity = 0;
    [SerializeField] private float hillVelocityModifier = 5;
    [SerializeField] private float maxVelocity = 20;
    [SerializeField] private float decreaseVelocityAmt = 3;
    [SerializeField] private LayerMask groundCheck;
    private bool isHalting = false;
    public Transform haltPoint;
    public float startingDistFromHalt;
    [SerializeField] private float startingHaltVelocity = 10;
    public float velocityWhenHaltWasSet;
    // [SerializeField] private GameObject testTransform;

    [Header("Model")]
    [SerializeField] private GameObject chaseModel;
    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> wheels;
    [SerializeField] private float wheelRotationModifier;
    [SerializeField] private GameObject pedalsRoot;
    [SerializeField] private Animator crankAnimator;
    [SerializeField] private Rig legRig;

    [Header("Mounting")]
    [SerializeField] private float distToMount;
    [SerializeField] private Transform externalPlayer;
    [SerializeField] private GameObject dismountPoint;
    private bool canDismount;

    [Header("Dialogue")]
    [SerializeField] private List<ChangeScript> bikeStub;
    [SerializeField] private List<ChangeScript> walkStub;
    private int subtitleIndex = 0;

    [Header("Test Features")]
    [SerializeField] private bool useTestCam;
    [SerializeField] private GameObject chaseCam;
    [SerializeField] private Canvas chaseCamUI;

    private int pedalHash;

    private float distanceAlongPath;
    private float currentVelocity;
    private bool lastFootWasRight;
    private bool stopped = false;
    private bool pedaling = false;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private float lastRightDot;

    private void Awake() {
        pedalHash = Animator.StringToHash("Pedal");

        if(chaseCamUI != null && chaseCam != null) {    
            chaseCamUI.gameObject.SetActive(false);
            chaseCam.gameObject.SetActive(false);
        }
    }

    private void Start() {
        transform.position = path.path.GetPointAtDistance(distanceAlongPath);

        chaseModel.SetActive(false);
    }

    private void Update() {
        if(!paused) {
            if(!isHalting) {
                if(InputManager.Instance) {
                    if(!pedaling) {
                        if(!stopped && !lastFootWasRight && InputManager.Instance.GetRightPedalPressed()) { // Check for Right Pedal
                            // Right Pedal
                            lastFootWasRight = true;
                            StartCoroutine(Pedal());
                        } else if(!stopped && lastFootWasRight && InputManager.Instance.GetLeftPedalPressed()) { // Check for Left Pedal
                            // Left Pedal
                            lastFootWasRight = false;
                            StartCoroutine(Pedal());
                            if(pedalUIPrompt.activeSelf) {
                                pedalCount++;
                                if(pedalCount >= promptDisablePedalAmt) {
                                    pedalUIPrompt.SetActive(false);
                                    pedalCount = 0;
                                }
                            }
                        }
                    }
                } else {
                    Debug.LogWarning("No Input Manager Detected");
                }

                if(!pedaling && currentVelocity != minVelocity) {
                    currentVelocity -= Time.deltaTime * decreaseVelocityAmt;
                    if(currentVelocity < minVelocity) {
                        currentVelocity = minVelocity;
                    }
                }

                if(currentVelocity < 0.1f) {
                    pedalTimer += Time.deltaTime;
                    if(pedalTimer >= timeForPedalPrompt) {
                        pedalUIPrompt.SetActive(true);
                        pedalCount++;
                    }
                } else {
                    pedalTimer = 0;
                }

            } else {
                float dist = Mathf.Abs(path.path.GetClosestDistanceAlongPath(haltPoint.position) - distanceAlongPath);
                if(dist > startingDistFromHalt * 0.8f) {
                    currentVelocity = Mathf.Lerp(velocityWhenHaltWasSet, startingHaltVelocity, (dist - startingDistFromHalt) / ((startingDistFromHalt * .8f) - startingDistFromHalt));
                } else {
                    // if(dist < startingDistFromHalt * 0.05f) {
                        currentVelocity = Mathf.Lerp(startingHaltVelocity, 0, (startingDistFromHalt - dist) / startingDistFromHalt);
                    /* }  else {
                        currentVelocity = 0;
                    } */
                    if(currentVelocity < 0.1f) {
                        canDismount = true;
                    }
                }
            }

            distanceAlongPath += currentVelocity * Time.deltaTime;

            // Update Position
            targetPosition = path.path.GetPointAtDistance(distanceAlongPath);
            RaycastHit hit;
            Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0), -transform.up, Color.red);
            Debug.DrawRay(transform.position - new Vector3(0, 0.3f, 0), transform.up, Color.green);
            if(Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), -transform.up, out hit, Mathf.Infinity, groundCheck)) {
                // ground is below player
                transform.position = new Vector3(targetPosition.x, hit.point.y, targetPosition.z);
            } else if(Physics.Raycast(transform.position - new Vector3(0, 0.3f, 0), transform.up, out hit, Mathf.Infinity, groundCheck)) {
                // ground is above player
                transform.position = new Vector3(targetPosition.x, hit.point.y, targetPosition.z);
            } else {
                transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            }
            // transform.position = targetPosition;

            // testTransform.transform.position = targetPosition;

            // if(currentVelocity > 0) {
                foreach(GameObject wheel in wheels) {
                    wheel.transform.Rotate(new Vector3(currentVelocity * wheelRotationModifier,0,0), Space.Self);
                }
            // }

            Vector3 lastRightRotation = transform.right;

            // Update Rotation
            transform.forward = path.path.GetDirectionAtDistance(distanceAlongPath);

            float dot = Vector3.Dot(transform.forward, Vector3.up);
            if(dot > 0.1f) {
                // 0.43 is the maximum 
                float lerp = dot / 0.43f;
                minVelocity = Mathf.Lerp(0, -hillVelocityModifier, lerp);
            } else if(dot < -0.1f) {
                float lerp = Math.Abs(dot) / 0.43f;
                minVelocity = Mathf.Lerp(0, hillVelocityModifier * 2, lerp);
            } else {
                if(minVelocity > 0) {
                    minVelocity -= Time.deltaTime;
                    if(minVelocity < 0) {
                        minVelocity = 0;
                    }
                } else if(minVelocity < 0) {
                    minVelocity += Time.deltaTime;
                    if(minVelocity > 0) {
                        minVelocity = 0;
                    }
                }
            }

            if(canDismount) {
                if(inputUIPanel) {
                    if(!inputUIPanel.activeSelf) {
                        inputUIPanel.SetActive(true);
                    }
                }
                if(InputManager.Instance.GetDismountInputPressed()) {
                    StartCoroutine(Dismount());
                }
            }

            if(useTestCam) {
                if(!chaseCamUI.gameObject.activeSelf) {
                    chaseCamUI.gameObject.SetActive(true);
                }
                if(!chaseCam.gameObject.activeSelf) {
                    chaseCam.gameObject.SetActive(true);
                }
            } else {
                if(chaseCamUI.gameObject.activeSelf) {
                    chaseCamUI.gameObject.SetActive(false);
                }
                if(chaseCam.gameObject.activeSelf) {
                    chaseCam.gameObject.SetActive(false);
                }
            }
        } else {
            // check for mounting
            if(canMount) {
                if(Vector3.Distance(transform.position, externalPlayer.position) < distToMount) {
                    // show UI if not showing
                    if(inputUIPanel) {
                        if(!inputUIPanel.activeSelf) {
                            inputUIPanel.SetActive(true);
                        }
                    }
                    if(InputManager.Instance.GetDismountInputPressed()) {
                        // Mount bike
                        StartCoroutine(Mount());
                    }
                }
            }
        }

        
    }

    private IEnumerator Pedal() {
        float startingVelocity = currentVelocity;
        float newVelocity = Mathf.Clamp(startingVelocity + pedalForce, minVelocity, maxVelocity);

        // float maxRot = pedalsRoot.transform.eulerAngles.x - 180;

        float lerp = 0;
        float time = GetTimeBetweenPedal();

        crankAnimator.SetTrigger(pedalHash);
        crankAnimator.speed = (1 / time);

        pedaling = true;
        while(lerp < 1) {
            lerp += (Time.deltaTime / time);
            if(lerp > 1) {
                lerp = 1;
            }
            currentVelocity = Mathf.Lerp(startingVelocity, newVelocity, lerp);

            /* if(pedalsRoot.transform.eulerAngles.x > maxRot) {
                pedalsRoot.transform.Rotate(new Vector3(-2 / time, 0, 0), Space.Self);
            } */

            yield return null;
        }
        pedaling = false;
        // pedalsRoot.transform.rotation = Quaternion.Euler(maxRot, pedalsRoot.transform.eulerAngles.y, pedalsRoot.transform.eulerAngles.z);
    }

    private IEnumerator Mount() {
        externalPlayer.GetComponent<ExternalController>().SetPaused(true);
        externalPlayer.gameObject.SetActive(false);
        paused = false;
        chaseModel.SetActive(true);
        animator.Play("OnBike");
        if(inputUIPanel) {
            inputUIPanel.SetActive(false);
        }
        shoulderCam.Priority += 100;
        // Set current dialogue
        if(subtitleIndex >= 0 && subtitleIndex < bikeStub.Count) {
            bikeStub[subtitleIndex].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        if(chaseCamUI != null && chaseCam != null && useTestCam) {
            chaseCamUI.gameObject.SetActive(true);
            chaseCam.gameObject.SetActive(true);
        }

        legRig.weight = 1;

        yield return new WaitForSeconds(1);
        
        pedalUIPrompt.SetActive(true);

        Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
    }

    private IEnumerator Dismount() {
        // Dismount bike

        legRig.weight = 0;

        if(chaseCamUI != null && chaseCam != null && useTestCam) {
            chaseCamUI.gameObject.SetActive(false);
            chaseCam.gameObject.SetActive(false);
        }

        animator.Play("OffBike");

        paused = true;
        // chaseModel.SetActive(false);
        if(inputUIPanel) {
            inputUIPanel.SetActive(false);
        }
        shoulderCam.Priority -= 100;

        yield return new WaitForSeconds(1f);

        chaseModel.SetActive(false);

        externalPlayer.gameObject.SetActive(true);
        externalPlayer.GetComponent<ExternalController>().SetAnimatorInControl(true);
        externalPlayer.transform.position = dismountPoint.transform.position;
        externalPlayer.transform.rotation = dismountPoint.transform.rotation;

        yield return new WaitForSeconds(0.5f);

        externalPlayer.GetComponent<ExternalController>().SetPaused(false);

        // Setting dialogue
        if(subtitleIndex >= 0 && subtitleIndex < walkStub.Count) {
            walkStub[subtitleIndex].gameObject.SetActive(true);
            subtitleIndex = -1;
        }

        Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
    }

    /* public void CrashDismount(Transform crashPoint) {
        StartCoroutine(CrashDismountRoutine(crashPoint));
    } */
    
    public IEnumerator CrashDismountRoutine(Transform point) {
        yield return Dismount();
        externalPlayer.transform.position = point.position;
        externalPlayer.transform.rotation = point.rotation;
    }

    private float GetTimeBetweenPedal() {
        return Mathf.Lerp(maxTimeBetweenPedal, minTimeBetweenPedal, (currentVelocity / maxVelocity));
    }

    public void SetIsPaused(bool isPaused) {
        paused = isPaused;
        if(paused) {
            chaseModel.SetActive(false);
        } else {
            chaseModel.SetActive(true);
        }
    }

    public void SetCanMount(bool _canMount) {
        canMount = _canMount;
    }

    public bool GetIsPaused() {
        return paused;
    }

    public void SetHaltingPoint(Transform haltDestination) {
        isHalting = true;
        haltPoint = haltDestination;
        startingDistFromHalt = Mathf.Abs(path.path.GetClosestDistanceAlongPath(haltDestination.position) - distanceAlongPath);
        velocityWhenHaltWasSet = currentVelocity;
    }
}
