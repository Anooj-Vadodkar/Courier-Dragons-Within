using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SBPScripts;
public class TempCutsceneOperations : MonoBehaviour
{
    [SerializeField] Image[] slideCollection;
    [SerializeField] ExternalController Chase;
    [SerializeField] GameObject carouselThreePoint;
    int index = 0;
    [SerializeField] float timeForEachSlide;
    [SerializeField] bool paused = true;
    [SerializeField] private float time = 1.0f;
    [SerializeField] bool cutsceneThree;
    [SerializeField] LightingManager lighting;
    [SerializeField] FadeOut fade;
    [SerializeField] CarouselController carousel;

    public void SetPaused(bool isPaused)
    {
        paused = isPaused;
    }

    private void Awake()
    {
        paused = false;
        time = timeForEachSlide;
        slideCollection[0].enabled = true;
        index++;
    }

    private void Update()
    {
        if (!paused)
        {
            time -= Time.deltaTime;
            if(time <= 0)
            {
                if (index >= slideCollection.Length)
                {
                    paused = true;
                    if (cutsceneThree)
                    {
                        slideCollection[slideCollection.Length-1].enabled = false;
                        EndCutscene();
                    }
                    else
                    {
                        this.gameObject.SetActive(false);
                    }
                    return;
                }
                
                    
                time = timeForEachSlide;
                slideCollection[index-1].enabled = false;
                slideCollection[index].enabled = true;
                index++;
                
            }
        }
    

    }

    public void EndCutscene()
    {
        if (cutsceneThree)
            StartCoroutine(CutsceneThree());
    }

    IEnumerator CutsceneThree()
    {
        fade.Fade();
        yield return new WaitForSeconds(4.0f);
        Chase.gameObject.transform.position = carouselThreePoint.gameObject.transform.position;
        Chase.gameObject.transform.rotation = carouselThreePoint.gameObject.transform.rotation;
        lighting.SetTimeOfDay(4.5f);
        fade.FadeIn();
        carousel.PlayCarousel();
        Chase.Sit();
        this.gameObject.SetActive(false);
        yield return null;
    }

}
