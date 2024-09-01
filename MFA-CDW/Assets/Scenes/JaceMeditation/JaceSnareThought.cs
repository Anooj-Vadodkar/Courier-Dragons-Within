using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Collider2D))]
public class JaceSnareThought : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 15;

    [SerializeField] public string text;

    private Vector3[] _pointPositions;
    private int _nextPoint = 0;
    private JaceSnare snare;
    private bool moving = true;

    public void SetPositions(Vector3[] positions, JaceSnare theSnare) { _pointPositions = positions; _nextPoint = 1; snare = theSnare; }

    void Update()
    {
        if(_nextPoint <= _pointPositions.Length)
        {
            if(_nextPoint > 0 && moving)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _pointPositions[_nextPoint], moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.localPosition, _pointPositions[_nextPoint]) < 1f)
                {
                    transform.localPosition = _pointPositions[_nextPoint];
                    _nextPoint++;
                    //transform.LookAt(_pointPositions[_nextPoint]);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        snare.RemoveThought(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MeditationPlayer>())
        {
            collision.gameObject.GetComponent<MeditationPlayer>().EnterThought(this);
            moving = false;
        }
    }
}
