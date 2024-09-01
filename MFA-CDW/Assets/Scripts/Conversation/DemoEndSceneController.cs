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

            if(_inputManager.GetDismountInputPressed())
            {
                dragon.SetActive(true);
                script.Unpause();
                reflect.SetActive(false);
                director.Play();
            }
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
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
