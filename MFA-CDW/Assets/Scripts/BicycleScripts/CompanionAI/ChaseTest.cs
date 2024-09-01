using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using PathCreation;
using UnityEngine.AI;
using UnityEngine.Windows;

namespace SBPScripts
{
    public class ChaseTest : BicycleMovement
    {

        List<Vector3> mCheckpoints = new List<Vector3>();
        [SerializeField] private int mCurrCheckpoint;
        bool mPedaling = false;
        [SerializeField] float slowDownDistance = 5.0f;
        float lastTime;

        public PathCreator road;
        //VertexPath path;
        float elapsed;
        float refreshPathTime = 1.0f;
        public Transform target;
        [SerializeField] float distanceFromPoint = 10.0f;
        Vector3 nextPointOnPath;
        [SerializeField] float aiInput = 0.5f;
        [SerializeField] float customAiInput = 1.0f;

        protected override void Start()
        {
            base.Start();
            road = FindObjectOfType<PathCreator>();
            nextPointOnPath = road.path.GetClosestPointOnPath(transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            //Figure out if you’re “close enough” to the next target point, and if you are,
            //advance your “next target point” index. Keep in mind the path will run out
            //as the Enemy takes laps, so don’t forget to wrap around back to 0

            float distFromNextPoint = (nextPointOnPath - transform.position).magnitude;
            if (distFromNextPoint < distanceFromPoint)
            {
                Vector3 lookAhead = transform.position + Vector3.Normalize(transform.forward) * 10;
                Vector3 tempTarget = road.path.GetClosestPointOnPath(lookAhead);
                nextPointOnPath = tempTarget;

            }


            //Debug.Log("forward: " + fPhysicsWheel.transform.forward + " next point: " + nextPointOnPath);
            //fPhysicsWheel.transform.forward = Vector3.Slerp(transform.forward, (nextPointOnPath - transform.position), 0.5f);

            float steerAmount = Vector3.Dot(fPhysicsWheel.transform.forward, Vector3.Normalize(nextPointOnPath - transform.position));

            float distanceToTarget = Vector3.Distance(transform.position, nextPointOnPath);
            //if (distanceToTarget <= slowDownDistance)
            if (false)
            {
                if (distanceToTarget > distanceFromPoint)
                {
                    Debug.Log("1");
                    // Calculate the velocity/acceleration needed to reach target with velocity of 0
                    float t = (distanceToTarget / slowDownDistance);
                    float a = -rb.velocity.magnitude / t;
                    float vel = rb.velocity.magnitude + a * (lastTime - t);
                    rb.velocity = vel * rb.velocity.normalized;
                    lastTime = t;

                    fPhysicsWheel.transform.forward = Vector3.Slerp(transform.forward, (nextPointOnPath - transform.position), 0.2f);
                }
                else
                {
                    Debug.Log("2");
                    rb.velocity = Vector3.zero;
                }
            }
            else
            {
                if (steerAmount >= 0.8f)
                {
                    float axis = CustomInput(aiInput, ref customAiInput, 5, 5, false);
                    SetTopSpeed(false);
                    MoveAIBike(aiInput, customAiInput);
                    fPhysicsWheel.transform.forward = Vector3.Slerp(transform.forward, (nextPointOnPath - transform.position), 0.4f);

                    //Debug.Log("aiInput " + aiInput + " customAIInput " + customAiInput + " wheel " + fPhysicsWheel.transform.forward);
                    // fPhysicsWheel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + customSteerAxis * steerAngle.Evaluate(rb.velocity.magnitude), 0);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.4f);
                    fWheelRb.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPointOnPath - transform.position), 0.4f);
                }
            }
        }
    }
}