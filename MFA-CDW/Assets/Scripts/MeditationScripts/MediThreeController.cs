using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class MediThreeController : MonoBehaviour
{
    [SerializeField] private int neededThoughts = 4;
    [SerializeField] private int neededBreaths = 3;
    [SerializeField] private float transitionMoveSpeed = 2.0f;
    [SerializeField] private float fadeInSpeed = 2.0f;
    [SerializeField] private GameObject thoughtsParent; 
    [SerializeField] private MediMovement player;
    [SerializeField] private Transform middleOfMaze;
    [SerializeField] private GameObject mazeOutline;
    [SerializeField] private Collider2D mazeCollider;
    [SerializeField] private GameObject mazeFrame;
    [SerializeField] private TextMeshProUGUI subtitles;
    [SerializeField] private List<ConfBulletSpawner> spawners;
    [SerializeField] private GameObject breathThoughtPrefab;
    [SerializeField] private RawImage whiteFade;
    [SerializeField] private GameObject mazeGates;
    public List<HomingThought> allBullets;
    private int breathsTaken = 0;
    private int thoughtCounter = 0;

    private bool inMaze = false;

    public void IncrementThoughtCounter() {
        allBullets = new List<HomingThought>();
        thoughtCounter++;
        if(thoughtCounter >= neededThoughts) {
            // Disable all thoughts
            thoughtsParent.SetActive(false);
            player.SetPaused(true);
            subtitles.text = "";
            breathsTaken++;
            StartCoroutine(BreathWave());
            StartCoroutine(MoveToCenter());
        }
    }

    private void Update() {
        if(!inMaze && InputManager.Instance.GetConfrontPressed()) {
            inMaze = true;
            StartCoroutine(TransitionToMaze());
        }
    }

    public void Breath(){
        breathsTaken++;
        StartCoroutine(BreathWave());
    }

    private IEnumerator BreathWave() {
        // Add bullets to list
        for(int i = 0; i < spawners.Count; i++) {
            foreach(GameObject bullet in spawners[i].SpawnWave()) {
                if(bullet.GetComponent<HomingThought>()) {
                    allBullets.Add(bullet.GetComponent<HomingThought>());
                }
            }
        }

        // un pause existing bullets
        foreach(HomingThought thought in allBullets) {
            thought.SetIsThoughtStopped(false);
        }

        // Speeding up
        float currentSpeed = 0;
        while(currentSpeed < 1) {
            foreach(HomingThought thought in allBullets) {
                thought.SetThoughtSpeed(currentSpeed * thought.GetMaxSpeed());
            }
            currentSpeed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(breathsTaken < neededBreaths) {

            yield return new WaitForSeconds(2.0f);

            ConfrontationBreathZone zone =  Instantiate(breathThoughtPrefab, player.transform.position, player.transform.rotation).GetComponentInChildren<ConfrontationBreathZone>();
            zone.onBreathed.AddListener(Breath);

            while(currentSpeed > 0) {
                foreach(HomingThought thought in allBullets) {
                    thought.SetThoughtSpeed(currentSpeed * thought.GetMaxSpeed());
                }
                currentSpeed -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            foreach(HomingThought thought in allBullets) {
                thought.SetIsThoughtStopped(true);
            }
        } else {
            StartCoroutine(TransitionToMaze());
        }
    }

    private IEnumerator MoveToCenter() {
        float lerp = 0;
        while(lerp < 1) {
            lerp += transitionMoveSpeed * Time.deltaTime;
            if(lerp > 1) {
                lerp = 1;
            }
            player.transform.position = Vector3.Lerp(player.transform.position, middleOfMaze.position, lerp);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator TransitionToMaze() {
        yield return new WaitForSeconds(2.0f);
        // fade to white

        float alpha = 0;
        while(alpha < 1) {
            whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, alpha);
            alpha += 1.0f * Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, alpha);
            }
            
            yield return new WaitForEndOfFrame();
        }
        mazeGates.SetActive(true);

        // enable collider
        mazeCollider.enabled = true;
        // mazeOutline.enabled = true;
        /* mazeOutline.GetComponent<SpriteRenderer>().enabled = true;
        mazeOutline.GetComponent<Collider2D>().enabled = true; */
        mazeOutline.SetActive(true);

        foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) {
            if(bullet.GetComponent<HomingThought>()) {
                StartCoroutine(bullet.GetComponent<HomingThought>().FadeOut());
            }
        }

        mazeOutline.GetComponent<SpriteRenderer>().color = new Color(mazeOutline.GetComponent<SpriteRenderer>().color.r,
                                                                    mazeOutline.GetComponent<SpriteRenderer>().color.g,
                                                                    mazeOutline.GetComponent<SpriteRenderer>().color.b, 1);

        yield return new WaitForSeconds(1.0f);

        while(alpha > 0) {
            whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, alpha);
            alpha -= Time.deltaTime;
            if(alpha < 0) {
                alpha = 0;
                whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, alpha);
            }
            
            yield return new WaitForEndOfFrame();
        }

        player.SetPaused(false);
        mazeFrame.SetActive(false);
    }
}
