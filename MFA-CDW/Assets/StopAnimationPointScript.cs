using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAnimationPointScript : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collisionInfo)

    {
        if (collisionInfo.gameObject.CompareTag("Belief"))//for this to work, we need to 'tag' gameObjects in unity
                                                     //that are meant to be obstacles as such by adding a new tag
                                                     //to the inspector under the object name name
        {
            //movement.enabled = false;
            Debug.Log("The Belief Has Arrived!");
            collisionInfo.gameObject.GetComponent<Animator>().enabled = false;
        }
    }
}
