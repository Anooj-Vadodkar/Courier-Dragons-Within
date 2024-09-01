using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpauseDialogue : MonoBehaviour
{
    [SerializeField] private ChangeScript scriptToUnpause;
    [SerializeField] private bool isUnpause = true;
    [SerializeField] private bool isActivate = false;
    [SerializeField] private ExternalTagController tagAI;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isUnpause && !isActivate)
                scriptToUnpause.Unpause();
            if (isUnpause && tagAI != null)
                tagAI.PlayWaveAnimation();
            if(isActivate && ! isUnpause)
                scriptToUnpause.gameObject.SetActive(true);
            
            //Debug.Log("Player passed through");
        }
                
    }

}
