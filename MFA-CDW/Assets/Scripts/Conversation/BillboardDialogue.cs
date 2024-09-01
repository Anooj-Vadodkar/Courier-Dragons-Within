using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BillboardDialogue : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private TMP_Text text;

    private float floatSpeed;
    private float floatDamper;
    private float startyPos;
    private float floatInc;
    private Vector3 rotateVector = new Vector3(0, 180, 0);
    private Vector3 positionVector;
    private void Awake() {
        text = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startyPos = transform.position.y;
        floatInc = 0;
        floatSpeed = 2.0f;
        floatDamper = 10.0f;
        positionVector = new Vector3(transform.position.x, startyPos + Mathf.Cos(floatInc) / floatDamper, transform.position.z);
    }

    public void Initialize(Transform changeTarget, string changeText) {
        text.text = changeText;
        target = changeTarget;
    }

    private void LateUpdate() {
        // Rotate towards target
        if(target != null) {
            transform.LookAt(target.transform);
            transform.Rotate(rotateVector, Space.Self);

            transform.position = positionVector;

            floatInc += floatSpeed * Time.deltaTime;
        }
    }
}
