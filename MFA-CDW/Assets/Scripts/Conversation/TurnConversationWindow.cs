using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnConversationWindow : MonoBehaviour
{

    [SerializeField]
    private Transform target;
    private Vector3 turnAround = new Vector3(0, 180, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
            transform.Rotate(turnAround, Space.Self);
        }

    }
}
