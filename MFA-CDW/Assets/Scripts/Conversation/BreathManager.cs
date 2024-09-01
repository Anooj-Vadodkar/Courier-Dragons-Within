using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBPScripts
{
    [RequireComponent(typeof(Slider))]
    public class BreathManager : MonoBehaviour
    { 
        private InputManager inputManager;

        private Slider breathSlider;

        [SerializeField]
        private float breathSpeed = 5.0f;
        [SerializeField]
        private CyclistAnimController animController;

        private bool reachedMax;

        // Start is called before the first frame update
        void Start()
        {
            inputManager = InputManager.Instance;
            breathSlider = GetComponent<Slider>();
        }

        // Update is called once per frame
        void Update()
        {
            if(animController) {
                if(animController.IsOnBike() && animController.IsDislodged()) {
                    if(inputManager.GetHopInput() != 0) {
                        breathSlider.value += breathSpeed * Time.deltaTime;
                        if(breathSlider.value >= breathSlider.maxValue) {
                            breathSlider.value = breathSlider.maxValue;
                            if(!reachedMax)
                                reachedMax = true;
                        }
                    } else {
                        breathSlider.value -= breathSpeed * Time.deltaTime;
                        if(breathSlider.value <= breathSlider.minValue) {
                            breathSlider.value = breathSlider.minValue;
                            if(reachedMax) {
                                reachedMax = false;
                                // restart Player
                                animController.RestartCyclist();
                                // breathSlider.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
