using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] [Tooltip("How long background take to fade in/out")] private float transitionTime = 1f;
    //what alpha each sprite should be at while active
    [SerializeField] private float baseOpacity = 1f;
    [SerializeField] private float sereneOpacity = 1f;
    [SerializeField] private float cacophonousHardOpacity = 1f;
    [SerializeField] private float capcophonousSoftOpacity = 0.15f;
    
    [SerializeField] private SpriteRenderer[] baseBackground;
    [SerializeField] private SpriteRenderer[] sereneBackground;
    [SerializeField] private SpriteRenderer[] cacophonousHardBackground;
    [SerializeField] private SpriteRenderer[] cacophonousSoftBackground;

    [Header("Testing Buttons")]
    [SerializeField] private bool switchToBase = false;
    [SerializeField] private bool switchToSerene = false;
    [SerializeField] private bool switchToCacophonous = false;

    //for testing
    private void OnValidate()
    {
        if (switchToBase)
        {
            TransitionToBase();
            switchToBase = false;
        }
        if (switchToSerene)
        {
            TransitionToSerene();
            switchToSerene = false;
        }
        if (switchToCacophonous)
        {
            TransitionToCacophonous();
            switchToCacophonous = false;
        }
    }

    //fades out each background in the array
    private void DeactivateBackground(SpriteRenderer[] backgrounds)
    {
        foreach (var sprite in backgrounds)
        {
            sprite.DOFade(0, transitionTime);
        }
    }

    public void TransitionToBase()
    {
        //deactivates other backgrounds
        DeactivateBackground(sereneBackground);
        DeactivateBackground(cacophonousHardBackground);
        DeactivateBackground(cacophonousSoftBackground);
        
        //activates this background
        foreach (var sprite in baseBackground)
        {
            sprite.DOFade(baseOpacity, transitionTime);
        }
    }
    
    public void TransitionToSerene()
    {
        //deactivates other backgrounds
        DeactivateBackground(baseBackground);
        DeactivateBackground(cacophonousHardBackground);
        DeactivateBackground(cacophonousSoftBackground);
        
        //activates this background
        foreach (var sprite in sereneBackground)
        {
            sprite.DOFade(sereneOpacity, transitionTime);
        }
    }
    
    public void TransitionToCacophonous()
    {
        //deactivates other backgrounds
        DeactivateBackground(sereneBackground);
        DeactivateBackground(baseBackground);
        
        //activates this background
        foreach (var sprite in cacophonousHardBackground) {
            sprite.DOFade(cacophonousHardOpacity, transitionTime);
        }

        foreach(var sprite in cacophonousSoftBackground) {
            sprite.DOFade(capcophonousSoftOpacity, transitionTime);
        }
    }
}
