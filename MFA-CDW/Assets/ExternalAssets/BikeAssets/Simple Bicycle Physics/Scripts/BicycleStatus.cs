using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Playables;

namespace SBPScripts
{
    public class BicycleStatus : MonoBehaviour
    {
        public bool onBike = true;
        [SerializeField]
        private float fadeInWaitTime = 2.0f;
        [SerializeField]
        private PlayableDirector director;
        [SerializeField]
        private GameObject fadeInScreen;
        [SerializeField]
        private ChangeScript convoNav;

        public bool dislodged;
        public float impactThreshold;
        public GameObject ragdollPrefab;
        [HideInInspector]
        public GameObject instantiatedRagdoll;
        bool prevOnBike, prevDislodged;
        public GameObject inactiveColliders;
        BicycleController bicycleController;
        Rigidbody rb;
        [SerializeField]
        private GameObject uiBreath;
        CameraController cameraController;
        [SerializeField]
        private GameObject walking2Debug;
        [SerializeField]
        private GameObject cycling1debug;
        private EventInstance bikeGear;
        private EventInstance bikeGravel;

        [SerializeField] private CustomBikeSounds customBikeSounds;

        void Start()
        {
            cameraController = CameraController.Instance;
            bicycleController = GetComponent<BicycleController>();
            // customBikeSounds = GetComponent<CustomBikeSounds>();
            Debug.Log(onBike);
            rb = GetComponent<Rigidbody>();
            if (onBike) {
                StartCoroutine(BikeStand(1));
            }
            else {
                StartCoroutine(BikeStand(0));
                StartCoroutine(FadeIn());
            }

            /* bikeGear = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.bikeGearSFX);
            bikeGear.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position));
            bikeGravel = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.bikeGravelSFX);
            bikeGravel.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position)); */
        }
        void OnCollisionEnter(Collision collision)
        {
            //Detects if there is a ragdoll to instantiate in the first place along with collsion impact detection
            if (collision.relativeVelocity.magnitude > impactThreshold && ragdollPrefab!=null) {
                dislodged = true;
                uiBreath.SetActive(true);
                if(cameraController.isInThirdPersonMode()) {
                    cameraController.GetThirdPersonCam().GetComponent<ThirdPersonCameraController>().ResetShake();
                }
            }
        }
        void Update()
        {
            if (Input.GetKeyDown("g"))
            {
                transform.position = walking2Debug.transform.position;
            }
            if (Input.GetKeyDown("k"))
                transform.position = cycling1debug.transform.position;
            if (onBike) {
                float speedVal = GetComponent<Rigidbody>().velocity.magnitude / GetComponent<BicycleMovement>().topSpeed; 

                // Debug.Log("Hit Adjust SFX: " + speedVal);
                bikeGear.setParameterByName("PAR_SX_BicycleGears_Intensity",speedVal * 10);
               // Debug.Log("Bike Gear Parameter: " + bikeGear.getParameterByName("PAR_SX_BicycleGears_Intensity"));
                bikeGravel.setParameterByName("PAR_SX_RidingOnGravel_Intensity",speedVal * 10);
            }

            if(dislodged != prevDislodged)
            {
                if(dislodged)
                {
                    bicycleController.fPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
                    bicycleController.rPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
                    bicycleController.rb.centerOfMass = bicycleController.GetComponent<BoxCollider>().center;
                    bicycleController.enabled = false;
                    inactiveColliders.SetActive(true);
                    instantiatedRagdoll = Instantiate(ragdollPrefab);
                }
                else
                {
                    bicycleController.fPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
                    bicycleController.rPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
                    bicycleController.enabled = true;
                    bicycleController.rb.centerOfMass = bicycleController.centerOfMassOffset;
                    inactiveColliders.SetActive(false);
                    Destroy(instantiatedRagdoll);
                }
            }
            prevDislodged = dislodged;
            if (onBike != prevOnBike)
            {
                if (onBike && dislodged == false)
                    StartCoroutine(BikeStand(1));
                else
                    StartCoroutine(BikeStand(0));
            }
            prevOnBike = onBike;
        }

        IEnumerator FadeIn() {
            // AudioManager.Instance.PlayEvent(FMODEvents.Instance.natureAmbient, Camera.main.transform.position);
            // AudioManager.Instance.SetAmbientEvent(FMODEvents.Instance.natureAmbient, true);
            fadeInScreen.SetActive(true);
            yield return new WaitForSeconds(fadeInWaitTime);
            GameObject.Find("ExternalPlayer").GetComponent<ExternalController>().PlayExitMedCutscene();
        }

        IEnumerator BikeStand(int instruction)
        {
            // Get on the bike
            if (instruction == 1)
            {
                AudioManager.Instance.PlayEvent(FMODEvents._instance.getOnBike, transform.position);

                customBikeSounds.enabled = true;
                customBikeSounds.StartEmitter();

                float t = 0f;
                while (t <= 1)
                {
                    t += Time.deltaTime * 5;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0),t);
                    yield return null;
                }
                bicycleController.enabled = true;
                rb.constraints = RigidbodyConstraints.None;
                
                
            }

            // Get off the bike
            if (instruction == 0)
            {
                //customBikeSounds.StartEmitter();
                //customBikeSounds.enabled = false;

                AudioManager.Instance.PlayEvent(FMODEvents._instance.getOffBike, transform.position);

                float t = 0f;
                while (t <= 1)
                {
                    t += Time.deltaTime * 5;
                    bicycleController.customSteerAxis = -Mathf.Abs(instruction - t);
                    yield return null;
                }
                bicycleController.enabled = false;
                yield return new WaitForSeconds(1);
                float l = 0f;
                while (l <= 1)
                {
                    l += Time.deltaTime * 5;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, l*5);
                    yield return null;
                }
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 5);
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

        }
    }
}
