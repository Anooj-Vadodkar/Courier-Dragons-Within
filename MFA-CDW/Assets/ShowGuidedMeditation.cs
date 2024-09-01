using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGuidedMeditation : MonoBehaviour
{
    [SerializeField]
    GameObject guidedmeditation;
    [SerializeField]
    GameObject thisGameObject;
    private void OnTriggerEnter2D(Collider2D collisionInfo)

    {
        if (collisionInfo.gameObject.CompareTag("Player"))//for this to work, we need to 'tag' gameObjects in unity
                                                          //that are meant to be obstacles as such by adding a new tag
                                                          //to the inspector under the object name name
        {
            //movement.enabled = false;
            Debug.Log("The player has hit " + thisGameObject.name);
            guidedmeditation.SetActive(true);
        }
    }
}
