using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorRotation : MonoBehaviour
{
    private Vector2 mousePos;
    private Vector3 dir;

    private void Update() {
        mousePos = Mouse.current.position.ReadValue();

        dir = (new Vector2(transform.position.x, transform.position.y) - mousePos).normalized;

        transform.rotation = Quaternion.Euler(dir);
    }
}
