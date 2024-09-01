using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAmbientTrigger : MonoBehaviour
{
    public void StopAmbient() {
        AudioManager.Instance.StopAmbient();
    }
}
