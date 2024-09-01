using PixelCrushers.DialogueSystem.SequencerCommands;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using Language.Lua;

public class PauseMenuController : MonoBehaviour
{
    public GameObject ControlsObject;
    public GameObject MenuObject;
    public GameObject PauseMenu;
    public GameObject PauseBackground;
    public GameObject UICanvas;
    public GameObject VistaCanvas;
    public Animator FadeOutAnimation;
    public Animator MenuFadeOutAnimation;
    public Animator LogoSlideAnimation;
    public Animator ControlsFadeIn;

    public Button primaryButton;
    public Button controlsButton;
    [SerializeField] private StudioEventEmitter pauseSnapshot;

    public static bool GameIsPause = false;
    public static bool ControlsObjectsActive = false;


    private void Start()
    {
        primaryButton.Select();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pause Button Presed");

            if (GameIsPause)
            {
                if (!ControlsObjectsActive)
                {
                    Resume();
                }
                else
                {
                 
                }
            }
            else
            {
                Pause();
            }
         
        }
    }


    public void Resume ()
        {
        pauseSnapshot.Stop();
        PauseMenu.SetActive(false);
        ControlsObject.SetActive(false);
        PauseBackground.SetActive(false);
        UICanvas.SetActive(true);
        VistaCanvas.SetActive(true);
        Time.timeScale = 1f;
        GameIsPause = false;
        }
        
    public void Pause()
    {
        pauseSnapshot.Play();
        primaryButton.Select();
        PauseMenu.SetActive(true);
        MenuObject.SetActive(true);
        PauseBackground.SetActive(true);
        UICanvas.SetActive(false);
        VistaCanvas.SetActive(false);
        Time.timeScale = 0f;
        GameIsPause = true;
    }


    public void Options()
    {
        LogoSlideAnimation.SetBool("LogoSlide", true);
    }


    public void Controls ()
    {
        
        ControlsObject.SetActive(true);
        LogoSlideAnimation.SetBool("LogoSlide", true);
        ControlsFadeIn.SetBool("ControllerFade", true);
        ControlsObjectsActive = true;
  
}

    public void BackToMenu()
    {
        if (ControlsObject.activeSelf == true )
        {
            ControlsObject.SetActive(false);
            ControlsFadeIn.SetBool("ControllerFade", false);
            controlsButton.Select();
            MenuObject.SetActive(true);
            LogoSlideAnimation.SetBool("LogoSlide", false);
            ControlsObjectsActive = false;
        }
    }

        public void PlayGame()
    {
        MenuFadeOutAnimation.SetBool("MenuFadeOut", true);
        StartCoroutine(Fading());
    }

    private IEnumerator Fading()
    {
        FadeOutAnimation.SetBool("Fade", true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }


    public void QuitGame()
    {
        Application.Quit();
    }

}


