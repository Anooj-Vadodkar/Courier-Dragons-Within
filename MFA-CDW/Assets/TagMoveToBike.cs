using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TagMoveToBike : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent tag;
    private Vector3 bikePoint = new Vector3(124.5f, 56.96f, 232.3f);
    public GameObject playerPosition;
    // Update is called once per frame
    void Update()
    {
      //  Debug.Log(Vector3.Distance(playerPosition.transform.position, transform.position));
        if (Vector3.Distance(playerPosition.transform.position, transform.position) > 20){
            
            tag.SetDestination(transform.position);
        } else
        {
            tag.SetDestination(bikePoint);
        }
        


    }
}
