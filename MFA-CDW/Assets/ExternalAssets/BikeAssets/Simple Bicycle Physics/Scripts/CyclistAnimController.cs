using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SBPScripts
{
    public class CyclistAnimController : MonoBehaviour
    {
        BicycleController bicycleController;
        [SerializeField] Animator anim;
        InputManager inputManager;
        CameraController cameraController;
        string clipInfoCurrent, clipInfoLast;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public bool isAirborne;
        [SerializeField]
        private ChangeScript[] bikeStub;
        [SerializeField]
        private ChangeScript[] walkStub;
        [SerializeField]
        private ExternalTagController externalAI;
        [SerializeField]
        private ExternalTagController secondExternalAI;
        [SerializeField]
        private GameObject bikeDismount;
        [SerializeField]
        private GameObject bikeMount;
        [SerializeField]
        private GameObject thirdPersonCamera;
        [SerializeField]
        private GameObject firstGate;
        private bool disableParkingWaypoint = false;
        [SerializeField] private ParticleSystem parkingWaypoint;
        [SerializeField]
        private int subtitleIndex = 0;

        public GameObject hipIK, chestIK, leftFootIK, leftFootIdleIK, rightFootIK, headIK;
        public GameObject chaseRig;
        BicycleStatus bicycleStatus;
        Rig rig;
        bool onOffBike;
        [Header("Character Switching")]
        [Space]
        public GameObject cyclist;
        public ExternalController externalCharacter;
        public GameObject externalCharacterCam;
        public Animator chaseModel;
        public GameObject demountPoint;
        public GameObject tagAI;
        float waitTime, prevLocalPosX;

        [SerializeField]
        private GameObject promptCanvas;
        [SerializeField]
        private GameObject promptText;

        private bool conversationOn = false;

        public bool canMount = false;
        private Vector3 AITeleport = new Vector3(797.25f, 36.54f, 708.99f);

        private TagBikingAI tagBikingAI;
        void Start()
        {
            inputManager = InputManager.Instance;
            cameraController = CameraController.Instance;
            bicycleController = FindObjectOfType<BicycleController>();
            bicycleStatus = FindObjectOfType<BicycleStatus>();
            rig = hipIK.transform.parent.gameObject.GetComponent<Rig>();
            if (bicycleStatus != null)
                onOffBike = bicycleStatus.onBike;
            if (cyclist != null)
                cyclist.SetActive(bicycleStatus.onBike);
            if (externalCharacter != null)
            {
                externalCharacter.gameObject.SetActive(!bicycleStatus.onBike);
/*                 if (!bicycleStatus.onBike)
                {
                    externalCharacter.GetComponent<ExternalController>().GetUp();
                } */
            }
            anim = GetComponent<Animator>();
            leftFootIK.GetComponent<TwoBoneIKConstraint>().weight = 0;
            leftFootIdleIK.GetComponent<TwoBoneIKConstraint>().weight = 0;
            chestIK.GetComponent<TwoBoneIKConstraint>().weight = 0;
            hipIK.GetComponent<MultiParentConstraint>().weight = 0;
            headIK.GetComponent<MultiAimConstraint>().weight = 0;

            tagBikingAI = FindObjectOfType<TagBikingAI>();
        }

        void Update()
        {
            GetOnBike();
            waitTime -= Time.deltaTime;
            waitTime = Mathf.Clamp(waitTime, 0, 1.5f);


            speed = bicycleController.transform.InverseTransformDirection(bicycleController.rb.velocity).z;
            isAirborne = bicycleController.isAirborne;
            anim.SetFloat("Speed", speed);
            anim.SetBool("isAirborne", isAirborne);
            if (bicycleStatus != null)
            {
                if (bicycleStatus.dislodged == false)
                {
                    if (!bicycleController.isAirborne && bicycleStatus.onBike)
                    {
                        clipInfoCurrent = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        if (clipInfoCurrent == "BikeIdleToStart" && clipInfoLast == "BikeIdle")

                            StartCoroutine(LeftFootIK(0));
                        if (clipInfoCurrent == "BikeIdle" && clipInfoLast == "BikeIdleToStart")
                            StartCoroutine(LeftFootIK(1));          
                        if (clipInfoCurrent == "BikeIdle" && clipInfoLast == "BikeReverse")
                            StartCoroutine(LeftFootIdleIK(0));
                        if (clipInfoCurrent == "BikeReverse" && clipInfoLast == "BikeIdle")
                            StartCoroutine(LeftFootIdleIK(0));

                        clipInfoLast = clipInfoCurrent;
                    }
                }
                else
                {
                    
                    cyclist.SetActive(false);
                }
            }
            else
            {
                if (!bicycleController.isAirborne)
                {
                    clipInfoCurrent = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    if (clipInfoCurrent == "IdleToStart" && clipInfoLast == "Idle")
                        StartCoroutine(LeftFootIK(0));
                    if (clipInfoCurrent == "Idle" && clipInfoLast == "IdleToStart")
                        StartCoroutine(LeftFootIK(1));
                    if (clipInfoCurrent == "Idle" && clipInfoLast == "Reverse")
                        StartCoroutine(LeftFootIdleIK(0));
                    if (clipInfoCurrent == "Reverse" && clipInfoLast == "Idle")
                        StartCoroutine(LeftFootIdleIK(1));

                    clipInfoLast = clipInfoCurrent;
                }
            }
            if (Input.GetKeyDown(KeyCode.T))
                canMount = true;
        }

        IEnumerator LeftFootIK(int offset)
        {
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.fixedDeltaTime;
                leftFootIK.GetComponent<TwoBoneIKConstraint>().weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                //leftFootIdleIK.GetComponent<TwoBoneIKConstraint>().weight = 1 - leftFootIK.GetComponent<TwoBoneIKConstraint>().weight;
                yield return null;
            }

        }
        IEnumerator LeftFootIdleIK(int offset)
        {
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.fixedDeltaTime;
                //leftFootIdleIK.GetComponent<TwoBoneIKConstraint>().weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                yield return null;
            }

        }
        IEnumerator AdjustRigWeight(int offset)
        {
            //StartCoroutine(LeftFootIK(1));
            if (offset == 0)
            {
                cyclist.SetActive(true);
                externalCharacter.gameObject.SetActive(false);
            }
            float t1 = 0f;
            while (t1 <= 1f)
            {
                t1 += Time.deltaTime;
                rig.weight = Mathf.Lerp(-0.05f, 1.05f, Mathf.Abs(offset - t1));
                yield return null;
            }
            if (offset == 1)
            {
                yield return new WaitForSeconds(0.2f);
                externalCharacter.gameObject.SetActive(true);
                externalCharacter.GetComponent<ExternalController>().GetUp();
                externalCharacter.SetAnimatorInControl(true);
                // Matching position and rotation to the best possible transform to get a seamless transition
                externalCharacter.transform.position = demountPoint.transform.position;
                externalCharacter.transform.rotation = Quaternion.Euler(externalCharacter.transform.rotation.eulerAngles.x, cyclist.transform.root.rotation.eulerAngles.y + 80, externalCharacter.transform.rotation.eulerAngles.z);
                cyclist.SetActive(false);
            }

        }

        public void RestartCyclist() {
            if(cameraController.isInThirdPersonMode()) {
                cyclist.SetActive(true);
            }
            bicycleStatus.dislodged = false;
        }

        public bool IsOnBike() {
            return bicycleStatus.onBike;
        }

        public bool IsDislodged() {
            return bicycleStatus.dislodged;
        }

        public void SetConversation(bool convo)
        {
            conversationOn = convo;
        }

        public void SetParkWaypoint(bool disable)
        {
            disableParkingWaypoint = disable;
        }
            
        public void GetOnBike()
        {
            if (cyclist != null && externalCharacter != null)
            {
                if (inputManager.GetDismountInputPressed() && bicycleController.transform.InverseTransformDirection(bicycleController.rb.velocity).z <= 10.0f && waitTime == 0)
                {
                    // Getting on Bike
                    if (!bicycleStatus.onBike)
                    {
                        if ((Vector3.Magnitude(externalCharacter.transform.GetChild(0).position - transform.position) < 6.0f) && canMount)
                        {

                            //AudioManager.Instance.SetMusicParam(MusicState.BIKING);
                            // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseMonologue, Camera.main.transform.position);
                            bikeMount.SetActive(false);
                            if (subtitleIndex >= 0 && subtitleIndex < bikeStub.Length)
                                bikeStub[subtitleIndex].gameObject.SetActive(true);
                            waitTime = 1.5f;
                            bicycleStatus.onBike = !bicycleStatus.onBike;
                            externalCharacter.gameObject.SetActive(false);
                            externalCharacter.spawnDialogue = false;
                            externalCharacterCam.SetActive(false);
                            promptCanvas.SetActive(false);
                            promptText.SetActive(false);
                            externalAI.SetFollowPlayer(false);
                            //externalAI.prefab.Find("CharacterTestModel").SetActive(false);
                            bicycleController.SetHalt(false);
                            bicycleController.SetDrag(0.1f);
                            //tagAI.SetActive(true);
                            thirdPersonCamera.SetActive(true);
                            if (prevLocalPosX < 0)
                                anim.Play("OnBike");
                            else
                                anim.Play("OnBikeFlipped");
                            tagBikingAI.isBiking = true;
                            StartCoroutine(AdjustRigWeight(0));
                        }
                        else
                        {
                            Debug.Log("Too far from bike: " + Vector3.Magnitude(externalCharacter.transform.GetChild(0).position - transform.position));
                        }
                    }
                    // Getting off bike
                    else
                    {
                        //convoStub.SetActive(true);
                        if(subtitleIndex >= 0 && subtitleIndex < walkStub.Length && bicycleController.GetHalt())
                        {
                            walkStub[subtitleIndex].gameObject.SetActive(true);
                            subtitleIndex = -1;
                        }
                        waitTime = 1.5f;
                        bicycleStatus.onBike = !bicycleStatus.onBike;
                        bikeDismount.SetActive(false);
                        
                        anim.Play("OffBike");
                        canMount = true;
                        secondExternalAI.SetDestination(firstGate.transform.position);
                        secondExternalAI.SetPaused(false);
                        if (disableParkingWaypoint)
                            parkingWaypoint.Stop();
                        StartCoroutine(AdjustRigWeight(1));
                        externalCharacter.transform.position = demountPoint.transform.position;
                        externalCharacter.gameObject.SetActive(false);
                        externalCharacterCam.SetActive(true);
                        promptText.SetActive(true);

                        //tagBikingAI.isBiking = false;
                    }
                }
                prevLocalPosX = externalCharacter.transform.localPosition.x;
            }
        }

        public void DismountPromptTrigger()
        {
            bikeDismount.SetActive(true);
        }

        public void SetBikingAI(TagBikingAI newBike)
        {
            tagBikingAI = newBike;
        }

        public void StartChaseTalking()
        {
            if (bicycleStatus.onBike)
            {
                Debug.Log("Start talking");
                anim.SetBool("isTalking", true);
            }
                
        }

        public void StopChaseTalking()
        {
            if (bicycleStatus.onBike)
            {
                Debug.Log("Stop talking");
                anim.SetBool("isTalking", false);
            }
                
        }
    }
}
