using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowTargetOnPath : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private CinemachineSmoothPath path;
    private CinemachineDollyCart cart;

    private void Awake() {
        cart = GetComponent<CinemachineDollyCart>();
    }

    // Update is called once per frame
    void Update()
    {
        cart.m_Position = path.FindClosestPoint(target.transform.position, 0, -1, 10);
    }
}
