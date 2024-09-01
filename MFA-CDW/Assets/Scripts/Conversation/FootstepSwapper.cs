using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class FootstepSwapper : MonoBehaviour
{
    private TerrainChecker checker;
    private string currentLayer = "";

    private EventInstance footstepEvent;
    private AudioManager am;

    // dirt = 0
    // grass = 1
    // sand = 2
    [Header("Terrain Names")]
    [SerializeField] private List<TerrainLayer> dirtTerrainLayers;
    [SerializeField] private List<TerrainLayer> grassTerrainLayers;
    [SerializeField] private List<TerrainLayer> sandTerrainLayers;

    private int matParamValue = 0;

    private float timer;
    private float maxTime = 0.25f;
    private bool footHit = false;

    [SerializeField] Transform leftFootPosition;
    [SerializeField] Transform rightFootPosition;

    void Start()
    {
        am = AudioManager.Instance; 
        // footstepEvent = am.CreateEventInstance(FMODEvents.Instance.footsteps);
        // RuntimeManager.AttachInstanceToGameObject(footstepEvent, transform);
        checker = new TerrainChecker();   
    }

    private void Update() {
        /*if(footHit) {
            timer += Time.deltaTime;
            if(timer > maxTime) {
                footHit = false;
                timer = 0;
            }
        }*/
    }
    
    public void PlayWalkEvent(int foot) {
        if(foot  == 1) //1 is left foot
        {
            CheckMat(leftFootPosition);
        }
        else //2 is right foot
        {
            CheckMat(rightFootPosition);
        }

        EventInstance footstepEvent = am.CreateEventInstance(FMODEvents.Instance.footsteps);
        RuntimeManager.AttachInstanceToGameObject(footstepEvent, transform);

        footstepEvent.setParameterByName("PAR_SFX_Footstep_Terrain_Switcher", matParamValue);

        footstepEvent.start();
        footstepEvent.release();
        /*if (!footHit) {
            footHit = true;
            CheckMat();

            EventInstance footstepEvent = am.CreateEventInstance(FMODEvents.Instance.footsteps);
            RuntimeManager.AttachInstanceToGameObject(footstepEvent, transform);

            footstepEvent.setParameterByName("PAR_SFX_Footstep_Terrain_Switcher", matParamValue);

            footstepEvent.start();
            footstepEvent.release();
        }*/
    }

    private void CheckMat(Transform footPos) {
        RaycastHit hit;
        if(Physics.Raycast(footPos.position, Vector3.down, out hit, 3))
        {
            // check if terrain exists
            if(hit.transform.GetComponent<Terrain>() != null) {
                Terrain t = hit.transform.GetComponent<Terrain>();
                // if layer matches our currentLayer
                if(currentLayer != checker.GetLayerName(transform.position, t)) {
                    currentLayer = checker.GetLayerName(transform.position, t);
                    // swap footsteps for FMOD!

                    if(CheckDirtSounds(currentLayer)) {
                        matParamValue = 0;
                    } else if(CheckGrassSounds(currentLayer)) {
                        matParamValue = 1;
                    } else if(CheckSandSounds(currentLayer)) {
                        matParamValue = 2;
                    }
                }
            }
        }
    }

    private bool CheckDirtSounds(string terrain) {
        foreach(TerrainLayer layer in dirtTerrainLayers) {
            if(layer.name.Equals(terrain))
                return true;
        }
        return false;
    }

    private bool CheckSandSounds(string terrain) {
        foreach(TerrainLayer layer in sandTerrainLayers) {
            if(layer.name.Equals(terrain))
                return true;
        }
        return false;
    }

    private bool CheckGrassSounds(string terrain) {
        foreach(TerrainLayer layer in grassTerrainLayers) {
            if(layer.name.Equals(terrain))
                return true;
        }
        return false;
    }
}
