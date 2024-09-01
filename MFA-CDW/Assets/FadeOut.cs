using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeOut : MonoBehaviour
{
    
    public CanvasGroup fade;
    public Image blackoutScreen;
    public bool fadeOut = false;
    [SerializeField] private ExternalController player;
    private Color alpha = new Color(0.0f, 0.0f, 0.0f, 0.01f);

    private void Update()
    {
        if(fadeOut && fade.alpha < 1)
        {
            fade.alpha += Time.deltaTime;
            if (fade.alpha >= 1) {
                fadeOut = false;
                StartCoroutine("FadeBlackout");
            }
        }
    }
    public void Fade()
    {
        player.SetPaused(true);
        fadeOut = true;
    }

    private IEnumerator FadeBlackout() {
        Debug.Log("Hit Blackout");
        yield return new WaitForSeconds(5.0f);
        Color c = blackoutScreen.color;
        float a = 0;
        while(a < 1) {
            a += Time.deltaTime;
            if(a > 1) {
                a = 1;
            }
            blackoutScreen.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
    
}
