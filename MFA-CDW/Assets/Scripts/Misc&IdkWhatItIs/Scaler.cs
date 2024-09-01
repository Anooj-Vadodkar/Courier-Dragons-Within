using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

[ExecuteAlways]
public class Scaler : MonoBehaviour
{
    [SerializeField]
    private bool flipped = false;

    private SpriteRenderer sp;

    private void Start() {
        sp = GetComponent<SpriteRenderer>();
        Resize();
    }

    private void Update() {
        Resize();
    }

    private void Resize() {
        if(sp != null) {
            float screenHeight = Camera.main.orthographicSize * 2.0f;
            float screenWidth = screenHeight * Screen.width / Screen.height;

            if(!flipped) {
                transform.localScale = new Vector3(
                    screenWidth / sp.sprite.bounds.size.x,
                    screenHeight / sp.sprite.bounds.size.y,
                    1f
                );
            } else {
                transform.localScale = new Vector3(
                    screenHeight / sp.sprite.bounds.size.y,
                    screenWidth / sp.sprite.bounds.size.x,
                    1f
                );
            }
        }
    }
}
