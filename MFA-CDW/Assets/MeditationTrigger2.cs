using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MeditationTrigger2 : MonoBehaviour
{
    public Volume volume;
    public float timeScale = 1f;
    private VolumeParameter<float> hueShift = new VolumeParameter<float>();
    public ColorAdjustments CA;
    
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet<ColorAdjustments>(out CA);
        if (CA == null)
            Debug.LogError("No ColorAdjustments found on profile");
    }

    // Update is called once per frame
    public void ChangeHue0()
    {
        if (CA == null)
        {
            return;
        }

        hueShift.value= 50;
        
        CA.hueShift.SetValue(hueShift);
    }
}
