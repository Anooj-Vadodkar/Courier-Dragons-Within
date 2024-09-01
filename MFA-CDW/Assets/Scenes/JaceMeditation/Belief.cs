using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Belief : MonoBehaviour
{
    [SerializeField] private Animation animationClip;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<MeditationPlayer>())
        {
            animationClip.Play(animationClip.clip.name);
            FindObjectOfType<MeditationPlayer>().gameObject.SetActive(false);
            foreach (var thought in FindObjectsOfType<ThoughtBullet>())
            {
                Destroy(thought.gameObject);
            }
        }
    }
}
