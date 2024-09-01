using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Point")]
public class SpawnPoint : ScriptableObject
{
    public Vector3 playerPoint;
    public Vector3 tagPoint;
    public Vector3 bikePoint;
    public TimeOfDay timeofDay;

    public enum TimeOfDay {
        DAY,
        EVENING,
        NIGHT,
        MORNING
    }
}
