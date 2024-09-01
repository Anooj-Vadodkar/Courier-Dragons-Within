using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class JaceSnare : MonoBehaviour
{
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;
    //LockToMouse playerMove;

    [SerializeField]
    private int spawnCount = 5;
    [SerializeField]
    private float spawnDelay = 2.0f;
    [SerializeField]
    private float startDelay = 1.5f;
    [SerializeField]
    private GameObject thoughtPrefab;

    private int numSpawned = 0;
    private Vector3[] pointPositions;
    private List<JaceSnareThought> snareThoughts = new List<JaceSnareThought>();
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        pointPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(pointPositions);
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if(numSpawned >= spawnCount && snareThoughts.Count == 0)
        {
            UnlockPlayer();
        }
    }

    public void BuildCollider()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        pointPositions = positions;
        List<Vector2> positions2 = new List<Vector2>();
        foreach(Vector3 pos in positions)
        {
            positions2.Add(pos);
        }
        edgeCollider.SetPoints(positions2);
    }

    private void UnlockPlayer()
    {
        //playerMove.Unlock();
        FindObjectOfType<MeditationPlayer>().canMove = true;
        gameObject.SetActive(false);
    }

    private void SpawnThought()
    {
        Debug.Log(pointPositions.Length);
        GameObject obj = Instantiate(thoughtPrefab, transform);
        obj.GetComponent<Transform>().localPosition = pointPositions[0];
        JaceSnareThought ST = obj.GetComponent<JaceSnareThought>();
        snareThoughts.Add(ST);
        ST.SetPositions(pointPositions, this);
        numSpawned++;
        if(numSpawned < spawnCount)
        {
            Invoke("SpawnThought", spawnDelay);
        }
    }

    public void RemoveThought(JaceSnareThought thought) { snareThoughts.Remove(thought); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MeditationPlayer>())
        {
            //playerMove = collision.gameObject.GetComponent<LockToMouse>();
            //playerMove.Lock();
            collision.gameObject.GetComponent<MeditationPlayer>().canMove = false;
            lineRenderer.enabled = true;
            if (spawnCount > 0)
            {
                Invoke("SpawnThought", startDelay);
            }
        }
    }

/* #if UNITY_EDITOR
    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }
#endif */
}


/* [CustomEditor(typeof(JaceSnare))]
public class JaceObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        JaceSnare myScript = (JaceSnare)target;
        if (GUILayout.Button("Build Collider"))
        {
            myScript.BuildCollider();
        }
    }
} */

