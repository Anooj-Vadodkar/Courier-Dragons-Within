using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;

namespace SBPScripts
{
    public class CompanionAI : BicycleMovement
    {
        public float RotationAmount = 2.0f;
        public int TicksPerSecond = 60;
        public bool Pause = false;

        [SerializeField]
        private PathCreator road;

        NavMeshAgent agent;
        NavMeshPath path;
        private int pathIndex;

        [SerializeField]
        private float refreshPathTime = 1.0f;
        private float elapsed;

        [SerializeField]
        private Transform target;

        private Vector3 roadPoint;

        [SerializeField]
        private float slowDownDistance = 5.0f;
        [SerializeField]
        private float minDistanceToTarget = 1.0f;
        [SerializeField]
        private AnimationCurve decelerationCurve;

        private Vector3 currentTarget;

        private float aiInput, customAiInput = 0;
        private float lastTime;
        [SerializeField]
        private float maxTime;

        protected override void Start()
        {
            base.Start();
            path = new NavMeshPath();
            agent = GetComponent<NavMeshAgent>();
            elapsed = 1.0f;
            lastTime = maxTime;
        }
        public void SetDestination(Transform destination)
        {
            target = destination;
        }
        private void Update()
        {
            // Defining Path
            elapsed += Time.deltaTime;
            if (elapsed > refreshPathTime)
            {
                elapsed -= 1.0f;
                agent.destination = target.position;
                roadPoint = road.path.GetClosestPointOnPath(target.position);
                NavMesh.CalculatePath(transform.position, roadPoint, NavMesh.AllAreas, path);
                if (path.corners.Length > 0)
                {
                    pathIndex = 1;
                    currentTarget = path.corners[pathIndex];
                    StopAllCoroutines();
                }
            }
            //Path Debug
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }

            float steerAmount = Vector3.Dot(fPhysicsWheel.transform.forward, Vector3.Normalize(currentTarget - transform.position));

            float distanceToTarget = Vector3.Distance(transform.position, roadPoint);
            if (distanceToTarget <= slowDownDistance)
            {
                //Debug.Log("here");
                if (distanceToTarget > minDistanceToTarget)
                {
                    // Calculate the velocity/acceleration needed to reach target with velocity of 0
                    float t = (distanceToTarget / slowDownDistance) * maxTime;
                    float a = -rb.velocity.magnitude / t;
                    float v = rb.velocity.magnitude + a * (lastTime - t);
                    rb.velocity = rb.velocity.normalized * v;
                    lastTime = t;

                    fPhysicsWheel.transform.forward = Vector3.Slerp(transform.forward, (currentTarget - transform.position), 0.5f);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }
            else
            {
                //Debug.Log("there");
                if (steerAmount >= 0.8f)
                {
                    aiInput = 0.5f;
                    CustomInput(aiInput, ref customAiInput, 5, 5, false);
                    SetTopSpeed(false);
                    MoveAIBike(aiInput, customAiInput);
                    //fPhysicsWheel.transform.forward = Vector3.Slerp(transform.forward, (currentTarget - transform.position), 0.5f);

                    Debug.Log("aiInput " + aiInput + " customAIInput " + customAiInput + " wheel " + fPhysicsWheel.transform.forward);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentTarget - transform.position), 0.4f);
                    fWheelRb.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentTarget - transform.position), 0.4f);
                }
            }

            /*             if(Vector3.Magnitude(transform.position - currentTarget) > minDistanceToTarget) {
                            MoveBike(aiInput, customAiInput);
                        } else {
                            pathIndex++;
                            if(pathIndex < path.corners.Length) {
                                currentTarget = path.corners[pathIndex];
                            }
                        } */
        }
    }
}
