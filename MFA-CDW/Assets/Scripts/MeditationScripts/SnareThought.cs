using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// [RequireComponent(typeof(Collider2D))]
public class SnareThought : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 15;

    private Vector3[] _pointPositions;
    private int _nextPoint = 0;
    private Transform rect;
    private Snare snare;

    public void SetPositions(Vector3[] positions, Snare theSnare) { _pointPositions = positions; _nextPoint = 1; snare = theSnare; }

/*     private void Start()
    {
        rect = GetComponent<Transform>();
    }
    void Update()
    {
        if(_nextPoint <= _pointPositions.Length)
        {
            if(_nextPoint > 0)
            {
                rect.localPosition = Vector3.MoveTowards(rect.localPosition, _pointPositions[_nextPoint], moveSpeed * Time.deltaTime);

                if (Vector3.Distance(rect.localPosition, _pointPositions[_nextPoint]) < 1f)
                {
                    rect.localPosition = _pointPositions[_nextPoint];
                    _nextPoint++;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyThought()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        snare.RemoveThought(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    } */
}
