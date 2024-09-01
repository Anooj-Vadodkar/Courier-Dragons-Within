using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject ControlsObject;
    public GameObject CreditsObject;
    public GameObject MenuObject;
    public Animator FadeOutAnimation;
    public Animator MenuFadeOutAnimation;
    public Animator LogoSlideAnimation;
    public Animator ControlsFadeIn;
    public Animator CreditsAnim;
    public Button primaryButton;
    public Button creditsButton;
    public Button controlsButton;


    private void Start()
    {
        primaryButton.Select();
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("MainMenu");
        }
    }


    public void Options()
    {
        LogoSlideAnimation.SetBool("LogoSlide", true);
    }

    public void Credits()
    {
        CreditsObject.SetActive(true);
        LogoSlideAnimation.SetBool("LogoSlide", true);
        CreditsAnim.SetBool("CreditsButtonClicked", true);
    }

    public void Controls ()
    {
        ControlsObject.SetActive(true);
        LogoSlideAnimation.SetBool("LogoSlide", true);
        ControlsFadeIn.SetBool("ControllerFade", true);
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
        }
        else if (CreditsObject.activeSelf == true)
        {
            CreditsObject.SetActive(false);
            CreditsAnim.SetBool("CreditsEnd", true);
            CreditsAnim.SetBool("CreditsButtonClicked", false);
            creditsButton.Select();
            LogoSlideAnimation.SetBool("LogoSlide", false);
            MenuObject.SetActive(true);
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


