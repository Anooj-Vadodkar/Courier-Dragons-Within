
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class localTriggerPrompt : MonoBehaviour
{
    private void Update()
    {
		TextMeshProUGUI textmeshPro = GetComponent<TextMeshProUGUI>();
	}

    
	private void OnCollisionEnter2D(Collision2D collisionInfo)

	{
		if (collisionInfo.collider.tag == "player")//for this to work, we need to 'tag' gameObjects in unity
													 //that are meant to be obstacles as such by adding a new tag
													 //to the inspector under the object name name
		{
			//Collision.GetComponent<>().enabled =true ;
			this.enabled = false;
		}
	}
}

