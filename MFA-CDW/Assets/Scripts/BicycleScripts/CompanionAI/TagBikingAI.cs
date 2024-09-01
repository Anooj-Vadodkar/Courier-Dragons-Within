using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using PathCreation;
using UnityEngine.AI;
using UnityEngine.Windows;
using UnityEngine.XR;

namespace SBPScripts
{
    public class TagBikingAI : BicycleMovement
    {
        public enum BikeState
        {
            Biking,
            Stop
        }



        List<Vector3> mCheckpoints = new List<Vector3>();
        [SerializeField] private int mCurrCheckpoint;
        bool mPedaling = false;
        float lastTime;

        public PathCreator road;
        //VertexPath path;
        float elapsed;
        float refreshPathTime = 1.0f;
        public Transform target;
        [SerializeField] float distanceFromPath = 10.0f;
        [SerializeField] float distanceFromPoint;
        Vector3 nextPointOnPath;
        [SerializeField] float aiInput = 0.5f;
        [SerializeField] float customAiInput = 1.0f;
        [SerializeField] float chaseMaxDistance = 10f;
        [SerializeField] Transform chaseTransform;
        [SerializeField] float minSteerAmount = 0.8f;
        [SerializeField] float baseTopSpeed;
        BikeState currBikeState;
        float mAccelerateTime;
        public float pathChangeY;
        public bool isBiking;
        Transform stop;
        protected override void Start()
        {
            base.Start();
            road = FindObjectOfType<PathCreator>();
            nextPointOnPath = road.path.GetClosestPointOnPath(transform.position);
            baseTopSpeed = topSpeed;
            currBikeState = BikeState.Biking;
        }

        // Update is called once per frame
        void Update()
        {
            if(nextPointOnPath == null)
            {
                isBiking = false;
            }

            if(!isBiking)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                return;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
            //Debug.Log(isBiking);

            float distFromNextPoint = (nextPointOnPath - transform.position).magnitude;
            if (distFromNextPoint < distanceFromPath)
            {
                Vector3 lookAhead = transform.position + Vector3.Normalize(transform.forward) * 10;
                Vector3 tempTarget = road.path.GetClosestPointOnPath(lookAhead);
                //Debug.Log("old point" + nextPointOnPath + "set new point " + tempTarget);
                pathChangeY = tempTarget.y - nextPointOnPath.y;
                if(pathChangeY <.001f)
                {
                    pathChangeY = .001f;
                }
                Debug.Log(pathChangeY);
                nextPointOnPath = tempTarget;
                

            }
            //Debug.Log("forward: " + fPhysicsWheel.transform.forward + " next point: " + nextPointOnPath);

            float steerAmount = Vector3.Dot(fPhysicsWheel.transform.forward, Vector3.Normalize(nextPointOnPath - transform.position));
            float distanceToTarget = 0;
            if (target != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
            }

            float distanceFromChase = Vector3.Distance(transform.position, chaseTransform.position);

            if (currBikeState == BikeState.Stop)
            {
                if (distanceToTarget > distanceFromPoint)
                {
                    Debug.Log("distanceToTarget: " + distanceToTarget);
                    Debug.Log("1");

                    mAccelerateTime += Time.deltaTime;
                    float accelerationMagnitude = Mathf.Lerp(rb.velocity.magnitude, 0,
                                                      mAccelerateTime / 2.0f);
                    rb.velocity += rb.velocity.normalized * accelerationMagnitude * Time.deltaTime;

                    fPhysicsWheel.transform.forward = 
                        Vector3.Slerp(transform.forward, (target.position - transform.position), 0.2f);
                }
                else
                {
                    Debug.Log("2");
                    rb.velocity = Vector3.zero;
                }
            }
            else if (currBikeState == BikeState.Biking)
            {
                /*//slow down or speed up according to chase's position
                if (distanceFromChase > (chaseMaxDistance + 2) && topSpeed >= 0)
                {
                    //Debug.Log("too far " + distanceFromChase); //slow down
                    topSpeed -= 10.0f;

                }
                else if (distanceFromChase < (chaseMaxDistance - 2))
                {
                    //Debug.Log("too close " + distanceFromChase); //speed up
                    topSpeed += 10.0f;
                }*/

                //if (steerAmount >= minSteerAmount)
                {
                    //Debug.Log("3");
                    float axis = CustomInput(aiInput, ref customAiInput, 5, 5, false);
                    SetTopSpeed(false);
                    MoveAIBike(aiInput, customAiInput);
                    fPhysicsWheel.transform.forward = 
                        Vector3.Slerp(transform.forward, (nextPointOnPath - transform.position), .2f);

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.2f);
                    fWheelRb.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.2f);


                }
                /*else
                {
                    //Debug.Log("4");
                    rb.velocity = Vector3.zero;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.2f);
                    fWheelRb.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.2f);
                }*/
            }
        }

        public void ChangeBikeState(BikeState newState)
        {
            currBikeState = newState;
        }

        public BikeState GetBikeState()
        {
            return currBikeState;
        }
    }
}