using SBPScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StateChangeCollider : MonoBehaviour
{
    [SerializeField] TagBikingAI bikingAI;
    float time;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (bikingAI.GetBikeState() == TagBikingAI.BikeState.Stop && time >= 10.0)
        {
            Debug.Log("start biking again");
            bikingAI.ChangeBikeState(TagBikingAI.BikeState.Biking);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bikingAI.ChangeBikeState(TagBikingAI.BikeState.Stop);
        time = 0;
    }
}
