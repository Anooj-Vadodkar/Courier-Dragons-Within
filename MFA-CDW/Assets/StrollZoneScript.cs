using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StrollZoneScript : MonoBehaviour
{
    [SerializeField]
    ExternalController externalControllerScript;
    [SerializeField]
    NavMeshAgent navMeshAgent;
    [SerializeField]
    float tagBaseSpeed;

    private void Start()
    {
        tagBaseSpeed = navMeshAgent.speed;
    }
    private void OnTriggerStay(Collider collisionInfo)

    {
        
        if (collisionInfo.GetComponent<Collider>().tag == "AI" && externalControllerScript != null)//for this to work, we need to 'tag' gameObjects in unity
                                                     //that are meant to be obstacles as such by adding a new tag
                                                     //to the inspector under the object name name
        {
            //movement.enabled = false;
            externalControllerScript.walkingZone = true;
        }

        if (collisionInfo.GetComponent<Collider>().tag == "Player" && navMeshAgent != null)
        {
           
            //movement.enabled = false;

            //clamp speed float by serialized variable.
            
            navMeshAgent.speed = Mathf.Clamp(navMeshAgent.speed, 0, 2);
        }
        

        }

    private void OnTriggerExit(Collider collisionInfo)
    {
        if (collisionInfo.GetComponent<Collider>().tag == "Player" && externalControllerScript != null)
        {
            //movement.enabled = false;
            externalControllerScript.walkingZone = false;
        }

        if(collisionInfo.GetComponent<Collider>().tag == "Player" && navMeshAgent != null)
        {
            navMeshAgent.speed = tagBaseSpeed;
        }
        return;
    }
}
