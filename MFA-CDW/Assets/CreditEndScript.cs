using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditEndScript : MonoBehaviour
{
    public GameObject gameManager;

    public void TriggerEndEvent()
    {
        gameManager.GetComponent<MenuController>().BackToMenu();
    }
}
