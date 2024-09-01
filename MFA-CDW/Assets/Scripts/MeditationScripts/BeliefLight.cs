using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeliefLight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightSprite;
    [SerializeField] private float maxOpacity = 1;
    [SerializeField] private float colorChangeTime = .2f;
    [SerializeField] private bool brightenOnBreathe = true; // whether light dims and brightens with the players breathing

    private float baseOpacity;

    private RingBreathe player;

    private void Start()
    {
        baseOpacity = lightSprite.color.a; // sets the base opacity to its current editor value
        player = FindObjectOfType<RingBreathe>();
    }

    private void Update()
    {
        if (brightenOnBreathe)
        {
            //brightens light when breathing
            if (player.BreathingIn || player.BreathingOut)
            {
                //brightens light
                ChangeOpacity(lightSprite.color.a, maxOpacity);
            }
            else
            {
                //returns light to default
                ChangeOpacity(lightSprite.color.a, baseOpacity);
            }
        }
    }

    private void ChangeOpacity(float start, float end)
    {
        //changes opacity over time
        StartCoroutine(LerpOpacity(start, end));
    }

    private IEnumerator LerpOpacity(float start, float end)
    {
        float currentOpacity = lightSprite.color.a;
        while ((currentOpacity <= end && currentOpacity < start) || currentOpacity >= end)
        {
            //calcs how much to change opacity by
            currentOpacity += (end - start) / (Time.deltaTime / colorChangeTime);
            Color newCol = lightSprite.color;
            newCol.a = currentOpacity;
            lightSprite.color = newCol;
            
            yield return new WaitForEndOfFrame();
        }
        Color finalCol = lightSprite.color;
        finalCol.a = end;
        lightSprite.color = finalCol;
    }
}
