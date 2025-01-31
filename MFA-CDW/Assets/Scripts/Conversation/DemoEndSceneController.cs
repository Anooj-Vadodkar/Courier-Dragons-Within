using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class DemoEndSceneController : MonoBehaviour
{
    [SerializeField]
    private bool playerInTriggerZone;
    private InputManager _inputManager;
    [SerializeField] private GameObject reflect;
    [SerializeField] private ChangeScript script;
    [SerializeField] private GameObject dragon;
    [SerializeField] private ExternalTagController tagController;
    [SerializeField] private Transform gate;
    public PlayableDirector director;

    private void Start()
    {
        _inputManager = InputManager.Instance;
    }

    void Update()
    {
        if (playerInTriggerZone)
        {
            // show UI for talk to tag

            if(_inputManager.GetDismountInputPressed()/*  && gate != null */)
            {
                if(gate != null) {
                    tagController.SetDestination(gate.position);
                    
                    /* dragon.SetActive(true);
                    script.Unpause();
                    reflect.SetActive(false);
                    director.Play(); */
                }
            }
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("Enter Demo End Zone");
            reflect.SetActive(true);
            playerInTriggerZone = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            reflect.SetActive(false);
            playerInTriggerZone = false;
        }
    }
}
