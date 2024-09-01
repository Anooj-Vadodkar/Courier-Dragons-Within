using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BeliefTuner : MonoBehaviour
{
    [FormerlySerializedAs("maxDistortion")] [SerializeField] private float tunedDistortion = 5f;
    [FormerlySerializedAs("minDistortion")] [SerializeField] private float unTunedDistortion = 0f;
    [SerializeField] private float distortionSpeed = 5f;
    [SerializeField] private float maxTuningDistance = 30f;
    [SerializeField] [Tooltip("How much does health affect tuning?")] [Range(0,1)]
    private float healthEffectiveness = 1f;
    [SerializeField] [Tooltip("How much does distance affect tuning?")] [Range(0,1)]
    private float distanceEffect = 1f;
    [SerializeField] private Color baseColor  = Color.yellow;
    [SerializeField] private Color tunedColor = Color.green;

    private Transform target;
    private MeditationPlayer player;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<MeditationPlayer>();
        target = player.target;
    }

    void Update()
    {
        ChangeDistortion();
        
        float currentAmount = sprite.material.GetFloat("_DistortionPlace");
        sprite.material.SetFloat("_DistortionPlace", currentAmount + distortionSpeed * Time.deltaTime);
        
        
    }

    private void ChangeDistortion()
    {
        float angleToBelief = Mathf.Atan2((target.position.y - transform.position.y), (target.position.x - transform.position.x)) * Mathf.Rad2Deg;
        float angleDiff = Mathf.DeltaAngle(angleToBelief, player.transform.eulerAngles.z);
        float closeness = Mathf.Abs(angleDiff/180);

        float playerHealth = player.mentalState;
        float distanceToBelief = (target.transform.position - player.transform.position).magnitude;
        float effectiveMaxDistance = maxTuningDistance * playerHealth * healthEffectiveness;

        closeness = 1 - closeness;
        closeness *= ((effectiveMaxDistance - distanceToBelief) / effectiveMaxDistance) * distanceEffect;
        closeness *= playerHealth * healthEffectiveness;
        
        if (distanceToBelief > effectiveMaxDistance)
        {
            sprite.material.SetFloat("_DistortionAmount", 0);
        }
        
        else
        {
            sprite.color = baseColor;
            sprite.material.SetFloat("_DistortionAmount", Mathf.Lerp(unTunedDistortion, tunedDistortion, closeness));
        }
    }
}
