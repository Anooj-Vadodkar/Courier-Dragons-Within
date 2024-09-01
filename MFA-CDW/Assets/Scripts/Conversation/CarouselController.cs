using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class CarouselController : MonoBehaviour
{
    public PlayableDirector director;
    public ExternalController player;

    [SerializeField] 
    private GameObject _exitPrompt;

    public bool playerInTriggerZone;
    public bool isCarouselActive;

    public bool showExitUI;

    private InputManager _inputManager;

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = InputManager.Instance;
        showExitUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U) && playerInTriggerZone) {
            //Play Carousel
            Debug.Log("Play Carousel");
            //player.SetPaused(true);
            director.Play();
            isCarouselActive = true;
        }

        if(showExitUI && isCarouselActive)
        {
            //Stop the timeline
            _exitPrompt.gameObject.SetActive(true);
            
        }

        if (_inputManager.GetDismountInputPressed())
        {
            StartCoroutine("LeaveCarousel");
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            playerInTriggerZone = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            playerInTriggerZone = false;
        }
    }

    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        // player.CanGetUp();
        // ^^ This line caused a bug where the standup prompt would appear after the player has already stood up
        Debug.Log("STOPPED");
        //player.SetPaused(false);
    }

    public void PlayCarousel()
    {
        director.Play();
        isCarouselActive = true;
    }

    public void StopCarousel()
    {
        player.GetUp();
        director.Stop();
        isCarouselActive = false;
    }
    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }

    public void ShowExitUI()
    {
        if(!showExitUI)
        {
            showExitUI = true;
        }
    }

    private IEnumerator LeaveCarousel()
    {
        yield return new WaitForSeconds(0.5f);
        //_exitPrompt.gameObject.SetActive(false);
        if(!player.GetStanding())
            player.GetUp();
        showExitUI = false;
        director.Stop();
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
