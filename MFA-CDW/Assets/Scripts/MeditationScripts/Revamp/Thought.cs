using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
// using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
// using Unity.VisualScripting;
using TMPro;
// using Microsoft.Unity.VisualStudio.Editor;
//using UnityEditor.Profiling;
using UnityEngine.UI;
using UnityEngine.Rendering.UI;
using FMODUnity;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System;

public class Thought : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI approachText;
    [SerializeField]
    private GameObject confrontationPoint;
    [SerializeField]
    private float radius = 4.5f;
    [SerializeField]
    private GameObject visionSphere;
    [SerializeField]
    private float visionMaxSize = 10.0f;
    [SerializeField]
    private float visionGrowSpeed = 5.0f;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Color angryColor;
    [SerializeField]
    private Color passiveColor;

    [SerializeField]
    private SpriteRenderer labyrinthSprite;
    [SerializeField]
    private SpriteRenderer emptyLabyrinthSprite;
    [SerializeField] private GameObject thoughtSprite;

    [SerializeField]
    private RawImage fadeOutBackground;
    
    [SerializeField]
    private float minCamSize = 3.0f;
    [SerializeField]
    private float maxCamSize = 25.0f;
    [SerializeField]
    private float minDistToThought = 2.0f;
    [SerializeField]
    private float maxDistToThought = 40.0f;

    [SerializeField]
    private BackgroundChanger backgroundChanger;

    private bool frag1Hit = false;
    private bool frag2Hit = false;
    private bool frag3Hit = false;
    private bool frag4Hit = false;

    [SerializeField]
    private GameObject player;

    private StudioEventEmitter thoughtsSFX;

    private AudioManager audioManager;

    private bool confronting = false;

    //breathing zones
    [SerializeField] private Transform safeZonesParent;
    [SerializeField] private GameObject narrativeCollisionGatesParent;
    private int remainingSafeZones;
    [SerializeField] private float breathZoneFadeInTime = 2f; //in seconds

    [SerializeField]
    public bool fakeOutThought = false;
    
    private Animator animator;
    private int moveHash;

    [SerializeField]
    private TextMeshProUGUI breathPromptText;
    [SerializeField]
    private Image inputPromptImage;

    private ConfBulletSpawner spawner;
    [SerializeField]
    private float transitionSpeed;

    [SerializeField]
    private EndMeditation endMeditation;

    [SerializeField]
    private ConfrontAtEnd endConfrontation;

    [SerializeField]
    private bool instantStop = true;
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = AudioManager.Instance;
        thoughtsSFX = GetComponent<StudioEventEmitter>();
        animator = GetComponent<Animator>();
        spawner = GetComponent<ConfBulletSpawner>();

        moveHash = Animator.StringToHash("Move");
        // remainingSafeZones = safeZonesParent.GetComponentsInChildren<ConfrontationBreathZone>().Length;
        remainingSafeZones = safeZonesParent.childCount;
        Debug.Log("Safe Zones:" + remainingSafeZones);
    }

    private void Update() {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        float distLerpVal = (distance - minDistToThought) / (maxDistToThought - minDistToThought);
        float camSize = Mathf.Lerp(minCamSize, maxCamSize, distLerpVal);

        if(confronting) {
            spriteRenderer.color = Color.Lerp(passiveColor, angryColor, distLerpVal);

            thoughtsSFX.SetParameter("PAR_SX_Thought_Lowpass", (1 - distLerpVal) * 2);
        } else if(InputManager.Instance.GetConfrontPressed()) {
            // narrativeCollisionGatesParent.SetActive(false);
            // disable all narrative elements
            for(int i = 0; i < narrativeCollisionGatesParent.transform.childCount; i++) {
                narrativeCollisionGatesParent.transform.GetChild(i).gameObject.SetActive(false);
            }
            StartCoroutine(StartConfrontation());
        }
    }

    public void Confronting() {
        if(!fakeOutThought) {
            if(audioManager == null) {
                audioManager = AudioManager.Instance;
            }
            // PlayerCursor.Instance.DisableCursor();
            audioManager.SetMusicParam(MusicState.MEDITATION_CONF);
            thoughtsSFX.Play();
            StartCoroutine(GrowVision(1));
            
            //activate safe zones
            safeZonesParent.gameObject.SetActive(true);
            foreach (var zone in safeZonesParent.GetComponentsInChildren<ConfrontationBreathZone>())
            {
                zone.FadeIn(breathZoneFadeInTime);
            }
            // spawner.SetSpawnerActive(true);
        } else {
            // start animation
            animator.SetTrigger(moveHash);
        }
    }

    public void SafeZoneBreathed(ConfrontationBreathZone breathZone)
    {
        if (confronting)
        {
            remainingSafeZones--;
            confrontationPoint.transform.position = breathZone.GetStartPos();
            if(remainingSafeZones == 0) {
                spawner.SetSpawnerActive(false);

                // Rough Exit
                if(instantStop)
                    endMeditation.Exit();
                if (!instantStop)
                {
                    endConfrontation.gameObject.SetActive(true);
                    endConfrontation.FadeIn(0.5f);
                }
                // Regular Exit
                // StartCoroutine(ExitMeditation());
                // StartCoroutine(GrowVision(-1));
            }
        }
    }

    public void BeginConfrontation()
    {
        StartCoroutine(StartConfrontation());
    }
    
    private IEnumerator ExitMeditation() {
        audioManager.SetMusicParam(MusicState.MEDITATION_SERENE);

        PlayerCursor.Instance.DisableCursor();

        confronting = false;

        backgroundChanger.TransitionToSerene();

        // Pause Player
        player.GetComponent<MediMovement>().SetPaused(true);

        thoughtsSFX.Stop();

        // Move towards player
        Vector3 startingPos = transform.position;
        float lerp = 0;
        float alpha = 1;
        Color startingColor = spriteRenderer.color;
        while(lerp < 2 && alpha > 0) {
            // transform.position = Vector3.Lerp(startingPos, player.transform.position, lerp);
            lerp += 1.0f * Time.deltaTime;
            if(lerp >= 1) {
                spriteRenderer.color = new Color(startingColor.r, startingColor.g, startingColor.b, alpha);
                alpha -= 1.0f * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }

        approachText.text = "Why would you...";
        StartCoroutine(FadeInApproachText());
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(2.0f);

        approachText.gameObject.SetActive(false);

        float alphaValue = 0;

        startingColor = inputPromptImage.color;

        breathPromptText.gameObject.SetActive(true);
        inputPromptImage.gameObject.SetActive(true);

        breathPromptText.text = "Leave ... When you are ready";

        while(alphaValue < 1) {
            breathPromptText.alpha = alphaValue;
            inputPromptImage.color = new Color(startingColor.r, startingColor.g, startingColor.b, alphaValue);
            alphaValue += 1 * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while(InputManager.Instance.GetBreathInput() == 0) {
            yield return new WaitForEndOfFrame();
        }

        // White Fade
        alpha = 0;
        startingColor = fadeOutBackground.color;
        while(alpha < 1) {
            fadeOutBackground.color = new Color(fadeOutBackground.color.r, fadeOutBackground.color.g, fadeOutBackground.color.b, alpha);
            alpha += 1.0f * Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                fadeOutBackground.color = new Color(fadeOutBackground.color.r, fadeOutBackground.color.g, fadeOutBackground.color.b, alpha);
            } else {
                yield return new WaitForEndOfFrame();
            }
        }
        audioManager.SetMusicParam(MusicState.CONVO_BASE);

        // Scene transition
        GameManager.Instance.SceneTranstition("Day1Scene_MWHVersion");
    }

    private IEnumerator GrowVision(int dir) {
        if(labyrinthSprite != null) {
            // NEW SYSTEM
            Color tempColor = labyrinthSprite.color;
            if(dir == 1) {
                StartCoroutine(FadeInApproachText());
                thoughtsSFX.Play();
                confronting = true;
            }  else if(dir == -1) {
                float alphaValue = 1;
                while(alphaValue > 0) {
                    tempColor.a = alphaValue;
                    alphaValue -= visionGrowSpeed * Time.deltaTime;
                    if(alphaValue < 0) {
                        tempColor.a = 0;
                    }
                    emptyLabyrinthSprite.color = tempColor;
                    yield return new WaitForEndOfFrame();
                }
                emptyLabyrinthSprite.gameObject.SetActive(false);
                StartCoroutine(FadeInApproachText());
            } 
        } else {
            // OLD SYSTEM
            if(dir == 1) {
                float visionSize = 0;
                visionSphere.transform.localScale = new Vector3(visionSize, visionSize, 0.1f);
                while(visionSize < visionMaxSize) {
                    visionSize += visionGrowSpeed * Time.deltaTime;
                    if(visionSize > visionMaxSize) {
                        visionSize = visionMaxSize;
                    }
                    visionSphere.transform.localScale = new Vector3(visionSize, visionSize, 0.1f);
                    yield return new WaitForEndOfFrame();
                }
                StartCoroutine(FadeInApproachText());
                thoughtsSFX.Play();
                confronting = true;
            } else if(dir == -1) {
                float visionSize = visionMaxSize;
                visionSphere.transform.localScale = new Vector3(visionSize, visionSize, 0.1f);
                while(visionSize > 0) {
                    visionSize -= visionGrowSpeed * Time.deltaTime;
                    if(visionSize < 0) {
                        visionSize = 0;
                    }
                    visionSphere.transform.localScale = new Vector3(visionSize, visionSize, 0.1f);
                    yield return new WaitForEndOfFrame();
                }
                StartCoroutine(FadeInApproachText());
            }
        }
    }

    private IEnumerator FadeInApproachText() {
        float alpha = 0;
        Color startingColor = approachText.color;

        // Fade In
        while(alpha < 1) {
            approachText.color = new Color(startingColor.r, startingColor.g, startingColor.b, alpha);
            alpha += 1 * Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                approachText.color = new Color(startingColor.r, startingColor.g, startingColor.b, alpha);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3.0f);

        // Fade Out
        while(alpha > 0) {
            approachText.color = new Color(startingColor.r, startingColor.g, startingColor.b, alpha);
            alpha -= 1 * Time.deltaTime;
            if(alpha < 0) {
                alpha = 0;
                approachText.color = new Color(startingColor.r, startingColor.g, startingColor.b, alpha);
            }
            yield return new WaitForEndOfFrame();
        }

        PlayerCursor.Instance.BaseRetical();
    }

    public IEnumerator StartConfrontation() {
        if(!fakeOutThought) {
            // confronting = true;
            backgroundChanger.TransitionToCacophonous();

            Confronting();
            player.GetComponent<MediMovement>().BeginConfronting(GetConfrontationPoint(), this);

            emptyLabyrinthSprite.gameObject.SetActive(true);

            float alpha = 0;
            Color startingColor = emptyLabyrinthSprite.color;
            startingColor.a = alpha;
            Color labyrinthColor = labyrinthSprite.color;

            // thoughtSprite.GetComponent<Animator>().enabled = false;
            if(thoughtSprite.GetComponent<Animator>()) {
                thoughtSprite.GetComponent<Animator>().SetTrigger(Animator.StringToHash("next"));
            }

            while(alpha < 1) {
                alpha += Time.deltaTime;
                if(alpha >= 1) {
                    alpha = 1;
                }

                // float scale = Mathf.Lerp(0.7f, 2.0f, alpha);
                // thoughtSprite.transform.localScale = new Vector3(scale, scale, scale);

                startingColor.a = alpha;
                labyrinthColor.a = 1 - alpha;
                emptyLabyrinthSprite.color = startingColor;
                labyrinthSprite.color = labyrinthColor;
                yield return null;
            }

            if(thoughtSprite.GetComponent<Animator>()) {
                thoughtSprite.GetComponent<Animator>().SetTrigger(Animator.StringToHash("next"));
            }

            labyrinthSprite.transform.parent.gameObject.SetActive(false);

            spawner.SetSpawnerActive(true);
            
            // Transition player to correct position
            Vector3 startingPos = transform.position;
            float lerp = 0;
            while(lerp < 1) {
                lerp += transitionSpeed * Time.deltaTime;
                if(lerp > 1)
                    lerp = 1;
                transform.position = Vector3.Lerp(startingPos, confrontationPoint.transform.position, lerp);

                yield return null;
            }

            // Start spawner
        } else {        
            Confronting();
        }
    }

    public Vector3 GetConfrontationPoint() {
        return confrontationPoint.transform.position;
    }

    public float GetRadius() {
        return radius;
    }

    public void EndMeditation()
    {
        endMeditation.Exit();
    }

    public Vector3 GetCenterPoint() {
        return visionSphere.transform.position;
    }

    public void DestroyThought() {
        Destroy(gameObject);
    }
}
