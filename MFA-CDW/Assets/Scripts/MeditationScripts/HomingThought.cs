using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class HomingThought : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float minDistToPlayer = 3.0f;
    [SerializeField] private float speed;
    [SerializeField] float rotationModifier;
    [SerializeField] private float turnSpeed = 20f;
    private Vector3 originalPosition;
    private float currentSpeed;
    private bool isPaused = false;

    private bool fading = false;

    private Vector2 currentDir = Vector2.zero;

    private void Start() {
        currentSpeed = speed;
        if(playerPos == null) {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartCoroutine(FadeIn());
    }

    private void Update() {
        if(!isPaused) {
            Vector3 vectorToTarget = playerPos.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);

            // Move towards player
            // Debug.Log("Current speed of " + gameObject.name + ": " + currentSpeed);
            transform.Translate(transform.right * currentSpeed * Time.deltaTime, Space.World);
            // transform.Translate(transform.right * 0 * Time.deltaTime, Space.World);

            if(!fading && Vector3.Distance(transform.position, playerPos.position) <= minDistToPlayer) {
                fading = true;
                StartCoroutine(FadeOut());
            }
        }
    }

    public void SetInitialDir(Vector3 startingDir) {
        currentDir = startingDir.normalized;  
    }

    public void AssignOrigin(Vector3 origin) {
        originalPosition = origin;
    }

    public void SetIsThoughtStopped(bool isStopped) {
        if(isStopped) {
            Debug.Log("Hit Stop Bullet");
            isPaused = true;
            currentSpeed = 0;
        } else {
            currentSpeed = speed;
            isPaused = false;
        }
    }

    public void SetThoughtSpeed(float setSpeed) {
        currentSpeed = setSpeed;
    }

    public float GetMaxSpeed() {
        return speed;
    }

    public float GetDistanceFromOrigin() {
        return Vector3.Distance(originalPosition, transform.position);
    }

    public Vector3 GetOrigin() {
        return originalPosition;
    }

    private IEnumerator FadeIn() {
        float a = 0;
        fading = true;
        Color originalColor = GetComponent<SpriteRenderer>().color;
        while(a < 1) {
            a += fadeSpeed * Time.deltaTime;
            if(a > 1) {
                a = 1;
            }
            GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            yield return new WaitForEndOfFrame();
        }
        fading = false;
    }

    public IEnumerator FadeOut() {
        Debug.Log("Hit Fade Out");
        float a = 1;
        fading = true;
        Color originalColor = GetComponent<SpriteRenderer>().color;
        while(a > 0) {
            a -= 4 * Time.deltaTime;
            if(a < 0) {
                Destroy(this.gameObject);
            }
            GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            yield return new WaitForEndOfFrame();
        }
        fading = false;
    }
}
