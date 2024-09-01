using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    Thought hostThought;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float slowAmt = 0.5f;
    [SerializeField]
    private float lifetime = 7.5f;
    [SerializeField]
    private GameObject hitParticlePrefab;
    private bool isEnabled = true;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D bulletCollider;
    [SerializeField] private float fadeOutSpeed = 1.0f;
    [SerializeField] private GameObject hitEffectPrefab;


    private float timer = 0;

    private GameObject player;

    private void Start() {
        // sprite = GetComponent<SpriteRenderer>();
        if(!bulletCollider) {
            bulletCollider = GetComponent<Collider2D>();
        }
    }

    private void Update() {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        if(timer > lifetime) {
            // Destroy(this.gameObject);
            StartCoroutine(FadeOut());
        }
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(isEnabled) {
            if(!collision.collider.CompareTag("Bullet")) {
                if(collision.collider.CompareTag("Player")) {

                    player = collision.collider.gameObject;

                    // Resents the player to the last breath zones they cleared.
                    // ResetPlayer(player);

                    // Slow player down
                    player.GetComponent<MediMovement>().SlowPlayer(slowAmt);

                    // create particle effect
                    Instantiate(hitEffectPrefab, player.transform);

                    Destroy(this.gameObject);
                } else if(collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Shield")) {
                    StartCoroutine(FadeOut());
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(isEnabled) {
            if(!collider.CompareTag("Bullet")) {
                if(collider.CompareTag("Shield") || collider.CompareTag("Wall")) {
                    StartCoroutine(FadeOut());
                } else if(collider.CompareTag("Player")) {
                    player = collider.gameObject;

                    // Resents the player to the last breath zones they cleared.
                    // ResetPlayer(player);

                    // Slow player down
                    player.GetComponent<MediMovement>().SlowPlayer(slowAmt);

                    // create particle effect
                    Instantiate(hitEffectPrefab, player.transform);

                    Destroy(this.gameObject);
                }
            }
        }
    }

    private IEnumerator FadeOut() {
        bulletCollider.enabled = false;
        float alpha = 1;
        Color newColor = sprite.color;
        while(alpha > 0) {
            alpha -= Time.deltaTime * fadeOutSpeed;
            newColor = new Color(newColor.r, newColor.g, newColor.b, alpha);
            sprite.color = newColor;
            yield return null;
        }
        Destroy(this.gameObject);
        yield return null;
    }

    public void DisableBullet() {
        isEnabled = false;
        StartCoroutine(FadeOut());
    }

    public void AssignThought(Thought thought) {
        hostThought = thought;
    }

    private void ResetPlayer(GameObject player) {
        player.transform.position = hostThought.GetConfrontationPoint();
        Instantiate(hitParticlePrefab, transform.position, Quaternion.Euler(0, 0, 0));
    }

    public void SetRotation(Vector3 upDirection) {
        transform.up = upDirection;
    }
}
