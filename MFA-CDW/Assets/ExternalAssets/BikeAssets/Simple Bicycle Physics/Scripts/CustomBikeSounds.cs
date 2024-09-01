using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SBPScripts;

public class CustomBikeSounds : MonoBehaviour
{
    private StudioEventEmitter bikeSoundsEmitter;
    private Rigidbody rb;
    private BicycleMovement bikeMovement;

    private float gravelIntensity = 0;
    private Vector2 playerInput;

    private void Start() {

        bikeSoundsEmitter = GetComponent<StudioEventEmitter>();
        rb = GetComponent<Rigidbody>();
        bikeMovement = GetComponent<BicycleMovement>();
    }

    private void Update() {
        gravelIntensity = rb.velocity.magnitude / bikeMovement.topSpeed;

        // check velocity of bike, if there is velocity . . .
        if(rb.velocity != Vector3.zero) {
            // play gravel sounds based on velocity
            bikeSoundsEmitter.SetParameter("PAR_SX_RidingOnGravel_Intensity", gravelIntensity);

            // check input from player
            playerInput = InputManager.Instance.GetPlayerMovement();

            if(playerInput.y > 0) {
                // if input is forward -> pedalling sounds
                bikeSoundsEmitter.SetParameter("PAR_SX_Bicycle_PlayerInput_Switcher", 0);
            } else if(playerInput.y == 0) {
                // if input is zero -> spoke sounds
                bikeSoundsEmitter.SetParameter("PAR_SX_Bicycle_PlayerInput_Switcher", 1);
            } else if(playerInput.y < 0) {
                // if input is back -> play nothing
                bikeSoundsEmitter.SetParameter("PAR_SX_Bicycle_PlayerInput_Switcher", 2);
            }
        } else {
            // if velocity is zero -> play nothing
            bikeSoundsEmitter.SetParameter("PAR_SX_Bicycle_PlayerInput_Switch", 2);
            bikeSoundsEmitter.SetParameter("PAR_SX_RidingOnGravel_Intensity", gravelIntensity);
        }
    }

    public void StartEmitter() {
        bikeSoundsEmitter.Play();
    }

    public void StopEmitter() {
        bikeSoundsEmitter.Stop();
    }
}
