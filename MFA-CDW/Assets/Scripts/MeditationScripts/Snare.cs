using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class Snare : MonoBehaviour
{
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;
    LockToMouse playerMove;

    [SerializeField]
    private int spawnCount = 5;
    [SerializeField]
    private float spawnDelay = 2.0f;
    [SerializeField]
    private float startDelay = 1.5f;
    [SerializeField]
    private float focusTime = 1.0f;
    [SerializeField]
    private GameObject thoughtPrefab;

    private int numSpawned = 0;
    private Vector3[] pointPositions;
    private List<SnareThought> snareThoughts = new List<SnareThought>();
    private Dictionary<SnareThought, float> snareThoughtTimes = new Dictionary<SnareThought, float>();
    private List<SnareThought> snareThoughtsToRemove = new List<SnareThought>();
    private GameObject player;
    private FieldOfView fov;

    private LimitingBelief limitingBelief;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        pointPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(pointPositions);
        ZeroOutZ(ref pointPositions);
        fov = FindObjectOfType<FieldOfView>();
        // lineRenderer.enabled = false;
    }

    private void Start()
    {
        // player.GetComponent<MediPlayerController>().Lock();
        // player.GetComponent<MeditationPlayer>().Lock();
        GameObject.FindObjectOfType<MeditationPlayer>().Lock();
        // lineRenderer.enabled = true;
        if (spawnCount > 0)
        {
            Invoke("SpawnThought", startDelay);
        }
    }

    private void LateUpdate()
    {
        if(numSpawned >= spawnCount && snareThoughts.Count == 0)
        {
            UnlockPlayer();
            limitingBelief.BreakSnare();
            return;
        }
        foreach (SnareThought thought in snareThoughts)
        {
            if(fov.thoughtsInView.Contains(thought.gameObject))
            {
                snareThoughtTimes[thought] += Time.deltaTime;
            }
            else if (snareThoughtTimes[thought] > 0.0f)
            {
                snareThoughtTimes[thought] = 0.0f;
            }
            if(snareThoughtTimes[thought] > focusTime)
            {
                snareThoughtsToRemove.Add(thought);
            }
        }

        while(snareThoughtsToRemove.Count > 0)
        {
            RemoveThought(snareThoughtsToRemove[snareThoughtsToRemove.Count - 1]);
            Destroy(snareThoughtsToRemove[snareThoughtsToRemove.Count - 1].gameObject);
            snareThoughtsToRemove.RemoveAt(snareThoughtsToRemove.Count - 1);
        }
    }

    public void BuildCollider()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        pointPositions = positions;
        ZeroOutZ(ref pointPositions);
        List<Vector2> positions2 = new List<Vector2>();
        foreach (Vector3 pos in positions)
        {
            positions2.Add(pos);
        }
        edgeCollider.SetPoints(positions2);
    }

    private void UnlockPlayer()
    {
        // player.GetComponent<MediPlayerController>().Unlock();
        GameObject.FindObjectOfType<MeditationPlayer>().Unlock();
        gameObject.SetActive(false);
    }

    private void SpawnThought()
    {
        GameObject obj = Instantiate(thoughtPrefab, transform);
        int dir = Random.Range(0, 1);
        obj.transform.localPosition = pointPositions[(pointPositions.Length - 1) * dir];
        SnareThought ST = obj.GetComponent<SnareThought>();
        snareThoughts.Add(ST);
        obj.name = "LimitingThought" + snareThoughts.Count;
        snareThoughtTimes.Add(ST, 0.0f);
        if(dir == 1)
        {
            Vector3[] array = pointPositions;
            System.Array.Reverse(array);
            ST.SetPositions(array, this);
        }
        else
        {
            ST.SetPositions(pointPositions, this);
        }
        numSpawned++;
        if(numSpawned < spawnCount)
        {
            Invoke("SpawnThought", spawnDelay);
        }
    }

    public void SetBelief(LimitingBelief belief) { limitingBelief = belief; }
    public void SetPlayer(GameObject obj) { player = obj; }
    public void RemoveThought(SnareThought thought) { snareThoughts.Remove(thought);
        snareThoughtTimes.Remove(thought);
    }
    public Vector3[] GetPointPositions() { return pointPositions; }

    private void ZeroOutZ(ref Vector3[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i].z = 0;
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.gameObject.CompareTag("Player"))
        {
            playerMove = collision.gameObject.GetComponent<LockToMouse>();
            if(playerMove != null) {
                playerMove.Lock();
            } else {
                collision.gameObject.GetComponent<MediPlayerController>().Lock();
            }
            // lineRenderer.enabled = true;
            if (spawnCount > 0)
            {
                Invoke("SpawnThought", startDelay);
            }
        }
    }*/

/* #if UNITY_EDITOR
    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }
#endif */
}


/* [CustomEditor(typeof(Snare))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Snare myScript = (Snare)target;
        if (GUILayout.Button("Build Collider"))
        {
            myScript.BuildCollider();
        }
    }
} */

