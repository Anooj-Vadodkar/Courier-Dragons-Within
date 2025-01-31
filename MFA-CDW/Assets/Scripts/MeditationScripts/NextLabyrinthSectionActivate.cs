using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NextLabyrinthSectionActivate : MonoBehaviour
{
    [SerializeField] SpriteRenderer currentLabyrinth;
    [SerializeField] SpriteRenderer nextLabyrinth;
    [SerializeField] SpriteRenderer finalSection;
    [SerializeField] bool fadeOut = false;
    [SerializeField] GameObject nextGate;
    Color currentLabyrinthColor;
    Color nextLabyrinthColor;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            StartCoroutine(FadeScreen());
            fadeOut = false;
        }
    }

    public void SetFadeOut(bool fadeVal)
    {
        fadeOut = fadeVal;
    }

    private IEnumerator FadeScreen()
    {
        //yield return new WaitForSeconds(2.0f);
        float a = 0;
        while(a < 1)
        {
            a += Time.deltaTime;
            if(a > 1)
                a = 1;
            currentLabyrinthColor = currentLabyrinth.color;
            nextLabyrinthColor = nextLabyrinth.color;

            currentLabyrinth.color = new Color(currentLabyrinthColor.r, currentLabyrinthColor.g, currentLabyrinthColor.b, 1 - a);
            if (finalSection != null)
                finalSection.color = new Color(currentLabyrinthColor.r, currentLabyrinthColor.g, currentLabyrinthColor.b, 1 - a);
            nextLabyrinth.color = new Color(nextLabyrinthColor.r, nextLabyrinthColor.g, nextLabyrinthColor.b, a);
            yield return null;
        }
        nextGate.SetActive(true);
    }
}
