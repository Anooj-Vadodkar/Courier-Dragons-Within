using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FadeOutFootsteps : MonoBehaviour
{
    public void FadeOut() {
        RuntimeManager.StudioSystem.setParameterByName("PAR_SFX_FOOTSTEP_FADEOUT", 1);
    }
}
