using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace SBPScripts
{
    [System.Serializable]
    public class CycleGeometry
    {
        public GameObject handles, lowerFork, fWheelVisual, RWheel, crank, lPedal, rPedal, fGear, rGear;
    }

    public class BicycleMovement : MonoBehaviour
    {
        [HideInInspector]
        public bool isBunnyHopping;
        [HideInInspector]
        public bool isAirborne;
        [HideInInspector]
        public bool isReversing;

        [Tooltip("Steer Angle Over Speed")]
        public AnimationCurve steerAngle;

        public AnimationCurve accelerationCurve;

        public AnimationCurve leanCurve;

        public CycleGeometry cycleGeometry;

        [HideInInspector]
        public float currentTopSpeed;

        public Vector3 centerOfMassOffset;

        [Range(0.1f, 0.9f)]
        [Tooltip("Ratio of Relaxed mode to Top Speed")]
        public float relaxedSpeed;
        public float reversingSpeed;
        public float topSpeed;
        public float torque;

        public GameObject fPhysicsWheel, rPhysicsWheel;
        [HideInInspector]
        public ConfigurableJoint fPhysicsWheelConfigJoint, rPhysicsWheelConfigJoint;

        [HideInInspector]
        public Rigidbody rb, fWheelRb, rWheelRb;

        TagBikingAI tagBikingAI;
        public float force;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();

            fWheelRb = fPhysicsWheel.GetComponent<Rigidbody>();
            fWheelRb.maxAngularVelocity = Mathf.Infinity;

            rWheelRb = rPhysicsWheel.GetComponent<Rigidbody>();
            rWheelRb.maxAngularVelocity = Mathf.Infinity;

            fPhysicsWheelConfigJoint = fPhysicsWheel.GetComponent<ConfigurableJoint>();
            rPhysicsWheelConfigJoint = rPhysicsWheel.GetComponent<ConfigurableJoint>();

            tagBikingAI = FindObjectOfType<TagBikingAI>();
        }

        public void SteerBike(float customSteerAxis, float customLeanAxis, float oscillationSteerEffect)
        {
            float turnLeanAmount = -leanCurve.Evaluate(customLeanAxis) * Mathf.Clamp(rb.velocity.magnitude * 0.1f, 0, 1);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, turnLeanAmount);

            // Physics based Steering Control.
            fPhysicsWheel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + customSteerAxis * steerAngle.Evaluate(rb.velocity.magnitude) + oscillationSteerEffect, 0);
            fPhysicsWheelConfigJoint.axis = new Vector3(1, 0, 0);
        }

        public void MoveBike(float rawCustomAccelerationAxis, float customAccelerationAxis)
        {
            float currentSpeed = rb.velocity.magnitude;



            if (currentSpeed < currentTopSpeed && rawCustomAccelerationAxis > 0)
            {
                rWheelRb.AddTorque(transform.right * torque * customAccelerationAxis);
            }

            // Moving forward
            if (currentSpeed < currentTopSpeed && rawCustomAccelerationAxis > 0 && !isAirborne && !isBunnyHopping)
            {
                rb.AddForce(transform.forward * accelerationCurve.Evaluate(customAccelerationAxis));
            }

            // Reversing
            if (currentSpeed < reversingSpeed && rawCustomAccelerationAxis < 0 && !isAirborne && !isBunnyHopping)
            {
                rb.AddForce(-transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * 0.5f);
            }

            CheckisReversing();

            // Breaking
            if (rawCustomAccelerationAxis < 0 && !isReversing && !isAirborne && !isBunnyHopping)
            {
                rb.AddForce(-transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * 2);
            }

            rb.centerOfMass = Vector3.zero + centerOfMassOffset;
        }

        public void MoveAIBike(float rawCustomAccelerationAxis, float customAccelerationAxis)
        {
            float currentSpeed = rb.velocity.magnitude;

            if (currentSpeed < currentTopSpeed && rawCustomAccelerationAxis > 0)
            {
                rWheelRb.AddTorque(transform.right * torque * customAccelerationAxis);
            }

            // Moving forward
            if (currentSpeed < currentTopSpeed && rawCustomAccelerationAxis > 0 && !isAirborne && !isBunnyHopping)
            {
                float slopeFactor = tagBikingAI.pathChangeY * 150000;
                rb.AddForce(transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * slopeFactor);
            }

            // Reversing
            if (currentSpeed < reversingSpeed && rawCustomAccelerationAxis < 0 && !isAirborne && !isBunnyHopping)
            {
                rb.AddForce(-transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * 0.5f);
            }

            CheckisReversing();

            // Breaking
            if (rawCustomAccelerationAxis < 0 && !isReversing && !isAirborne && !isBunnyHopping)
            {
                rb.AddForce(-transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * 2);
            }

            rb.centerOfMass = Vector3.zero + centerOfMassOffset;
        }

        public float CustomInput(float r, ref float axis, float sensitivity, float gravity, bool isRaw)
        {
            var s = sensitivity;
            var g = gravity;
            var t = Time.unscaledDeltaTime;

            if (isRaw)
            {
                axis = r;
            }
            else
            {
                if (r != 0)
                    axis = Mathf.Clamp(axis + r * s * t, -1f, 1f);
                else
                    axis = Mathf.Clamp01(Mathf.Abs(axis) - g * t) * Mathf.Sign(axis);
            }

            return axis;
        }

        public void SetTopSpeed(bool sprinting)
        {
            if (!sprinting)
            {
                currentTopSpeed = topSpeed;
                //currentTopSpeed = Mathf.Lerp(currentTopSpeed, topSpeed * relaxedSpeed, Time.deltaTime);
            }
            else
            {
                currentTopSpeed = Mathf.Lerp(currentTopSpeed, topSpeed, Time.deltaTime);
            }
        }

        public void CheckisReversing()
        {
            isReversing = (transform.InverseTransformDirection(rb.velocity).z < 0) ? true : false;
        }
    }
}
