using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private LayerMask fovMask;
    [SerializeField]
    private int fovRayCount = 50;
    [SerializeField]
    private float viewDistance = 25.0f;
    [SerializeField]
    private float viewGrowSpeed = 5.0f;
    private float currentDistance = 0.0f;

    [SerializeField][Range(2, 10)]
    private int fovColliderAccuracy = 10;

    // public Dictionary<SnareThought, GameObject> thoughtsInView = new Dictionary<SnareThought, GameObject>();
    public List<GameObject> thoughtsInView = new List<GameObject>();

    private Mesh mesh;
    private PolygonCollider2D polygonCollider;

    [SerializeField]
    private float fovChangeSpeed = 10.0f;

    [SerializeField]
    private MediMovement player;

    [SerializeField]
    private BackgroundChanger backgroundChanger;

    private Vector3 origin = Vector3.zero;
    float fov = 90.0f;
    private float minFov = 4.0f;
    float maxFov = 90.0f;
    private float startingAngle = 0.0f;

    private InputManager inputManager;
    private GameManager gameManager;

    private bool paused = false;
    private bool ready = false;
    private bool focusing = false;

    [SerializeField]
    private float cursorSpeed = 5.0f;

    private void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polygonCollider = GetComponent<PolygonCollider2D>();

        inputManager = InputManager.Instance;
        gameManager = GameManager.Instance;
    }

    // Generates the field of view mesh every frame
    private void FixedUpdate() {
        if(!paused) {
            if(ready) {
                if(inputManager.GetBreathInput() == 1) {
                    if(thoughtsInView.Count != 0) {
                        // transition to reticle 3 if not already on reticle 3
                        PlayerCursor.Instance.BreathingReticle();
                        PlayerCursor.Instance.SetSpeed(cursorSpeed);
                    } else {
                        // transition to reticle 1 if not already on reticle 1
                        PlayerCursor.Instance.BaseRetical();
                        PlayerCursor.Instance.ResetRotation();
                    }

                    focusing = true;
                    if(fov > minFov) {
                        fov -= fovChangeSpeed * Time.deltaTime;
                    }
                    if(fov < minFov) {
                        fov = minFov;
                        // Enter Confrontation
                        if(thoughtsInView.Count != 0) {
                            StartCoroutine(StartConfrontation(thoughtsInView[0]));
                        }
                    }
                } else {
                    if(thoughtsInView.Count != 0) {
                        PlayerCursor.Instance.PromptRetical();
                        PlayerCursor.Instance.ResetRotation();
                    } else {
                        // transition to reticle 1 if not already on reticle 1
                        PlayerCursor.Instance.BaseRetical();
                        PlayerCursor.Instance.ResetRotation();
                    }

                    focusing = false;
                    if(fov < maxFov) {
                        fov += fovChangeSpeed * Time.deltaTime;
                    }
                    if(fov > maxFov) {
                        fov = maxFov;
                    }
                }
                // cursorSprite.transform.Rotate(new Vector3(0,0,20) * currentCursorSpeed * Time.deltaTime);
            }
            CreateFOV(fov);
        }
    }

    private void CreateFOV(float fov) {
        float angle = 0;
        float setAngle = 0;
        if(focusing && thoughtsInView.Count != 0) {
            Vector2 dirToThought = (thoughtsInView[0].transform.position - origin).normalized;
            float dirAngle = Mathf.Atan2(dirToThought.y, dirToThought.x) * Mathf.Rad2Deg;
            if(dirAngle < 0)
                dirAngle += 360;

            setAngle = dirAngle;
        } else {
            setAngle = startingAngle;
        }

        angle = setAngle;

        float angleIncrease = fov / fovRayCount;

        Vector3[] vertices = new Vector3[fovRayCount + 2];
        Vector2[] colliderPath = new Vector2[fovColliderAccuracy + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[fovRayCount * 3];

        vertices[0] = origin;
        colliderPath[0] = origin;

        int vertexIndex = 1;
        int colliderIndex = 1;
        int triangleIndex = 0;

        int colliderCheckInc = fovRayCount / fovColliderAccuracy;
        int currentColliderCheck = 0;

        if(currentDistance < viewDistance) {
            currentDistance += (viewGrowSpeed * Time.deltaTime);
            if(currentDistance > viewDistance) {
                currentDistance = viewDistance;
                ready = true;
                player.SetPaused(false);
            }
        }

        for(int i = 0; i <= fovRayCount / 2; i++) {
            // converts angle from degrees to radians
            float angleRad = angle * (Mathf.PI / 180);
            // covnerts the angle into a direction in the form of a vector3
            Vector3 direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            // Calculates the position of the vertex by extending from the origin in the direction of the angle for a given distance
            Vector3 vertex;

            RaycastHit2D raycastHit = Physics2D.Raycast(origin, direction, currentDistance, fovMask);
            if(raycastHit.collider == null) {
                // No Hit
                vertex = origin + direction * currentDistance;
            } else {
                // Hit
                vertex = raycastHit.point;
            }

            vertices[vertexIndex] = vertex;

            if(currentColliderCheck == 0 || i == fovRayCount) {
                colliderPath[colliderIndex] = vertex;
                colliderIndex++;
                currentColliderCheck = colliderCheckInc;
            } else {
                currentColliderCheck -= 1;
            }

            // initializes the triangle
            if(i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        angle = setAngle;

        for(int i = 0; i < fovRayCount / 2; i++) {
            // converts angle from degrees to radians
            float angleRad = angle * (Mathf.PI / 180);
            // covnerts the angle into a direction in the form of a vector3
            Vector3 direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            // Calculates the position of the vertex by extending from the origin in the direction of the angle for a given distance
            Vector3 vertex;

            RaycastHit2D raycastHit = Physics2D.Raycast(origin, direction, currentDistance, fovMask);
            if(raycastHit.collider == null) {
                // No Hit
                vertex = origin + direction * currentDistance;
            } else {
                // Hit
                vertex = raycastHit.point;
            }

            vertices[vertexIndex] = vertex;

            if(currentColliderCheck == 0 || i == fovRayCount) {
                colliderPath[colliderIndex] = vertex;
                colliderIndex++;
                currentColliderCheck = colliderCheckInc;
            } else {
                currentColliderCheck -= 1;
            }

            // initializes the triangle
            if(i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex;
                triangles[triangleIndex + 2] = vertexIndex - 1;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle += angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Sets the path for the polygon collider
        polygonCollider.SetPath(0, colliderPath);
    }

    public Mesh GetMesh() { return mesh; }
    public void SetOrigin(Vector3 origin) {
        this.origin = origin;
        mesh.RecalculateBounds();
    }

    // Called from 'MediPlayerController' to set the aim of the fov to where the cursor is
    public void SetAimDirection(Vector2 aimDirection) {
        aimDirection = aimDirection.normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        if(angle < 0)
            angle += 360;
        // startingAngle = angle - fov / 2.0f + 90;
        startingAngle = angle;
        // startingAngle = Mathf.Lerp(startingAngle, angle, 0.2f);
    }

    // When a bullet comes in view, added to list
    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.CompareTag("Belief")) {
            // Check if the list already contains this gameobject
            foreach (GameObject thought in thoughtsInView) {
                if(thought == collider.gameObject)
                    return;
            }

            thoughtsInView.Add(collider.gameObject);
        }
    }

    // When a bullet leaves view, remove from list
    private void OnTriggerExit2D(Collider2D collider) {
        if(collider.gameObject.CompareTag("Belief")) {
            thoughtsInView.Remove(collider.gameObject);
        }
    }

    public void ClearThoughtCache() {
        thoughtsInView.Clear();
    }

    public void SetPaused(bool isPaused) {
        paused = isPaused;
    }

    private IEnumerator StartConfrontation(GameObject thought) {
        if(!thought.GetComponent<Thought>().fakeOutThought) {
            backgroundChanger.TransitionToCacophonous();

            currentDistance = 0;
            CreateFOV(fov);
            paused = true;

            thought.GetComponent<Thought>().Confronting();
            player.BeginConfronting(thought.GetComponent<Thought>().GetConfrontationPoint(), thought.GetComponent<Thought>());

            yield return new WaitForSeconds(10.0f);
        } else {        
            thought.GetComponent<Thought>().Confronting();
        }
    }
}
