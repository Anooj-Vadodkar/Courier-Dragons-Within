using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MeditationSubtitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI centerSubtitleUI;
    [SerializeField] private TextMeshProUGUI rightSubtitleUI;
    [SerializeField] private TextMeshProUGUI leftSubtitleUI;

    // private List<string> currentText = new List<string>();
    private List<Subtitle> currentText = new List<Subtitle>();
    [SerializeField] private MediMovement player;

    [System.Serializable]
    public class Subtitle {
        public List<string> line = new List<string>{"test"};
        public float voTime;
        public SubtitleLocation location;
    }

    public enum SubtitleLocation {
        LEFT,
        RIGHT,
        CENTER,
        BOTH
    }

    private void Advance()
    {
        if (currentText.Count > 0)
        {
            switch(currentText[0].location) {
                case(SubtitleLocation.CENTER):
                    centerSubtitleUI.text = currentText[0].line[0];
                    leftSubtitleUI.text = "";
                    rightSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.RIGHT):
                    rightSubtitleUI.text = currentText[0].line[0];
                    leftSubtitleUI.text = "";
                    centerSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.LEFT):
                    leftSubtitleUI.text = currentText[0].line[0];
                    centerSubtitleUI.text = "";
                    rightSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.BOTH):
                    leftSubtitleUI.text = currentText[0].line[0];
                    rightSubtitleUI.text = currentText[0].line[1];
                    centerSubtitleUI.text = "";
                    break;
            }
            Invoke("Advance", currentText[0].voTime);
            currentText.RemoveAt(0);
        }
        else
        {
            leftSubtitleUI.text = "";
            centerSubtitleUI.text = "";
            rightSubtitleUI.text = "";
            player.SetPaused(false);
        }
    }

    public void DisplaySubtitle(Subtitle[] text, bool pausePlayer)
    {
        if(pausePlayer) {
            //player.SetPaused(true);
        }
        currentText = text.ToList();
        if(currentText.Count > 0) {
            switch(currentText[0].location) {
                case(SubtitleLocation.CENTER):
                    centerSubtitleUI.text = currentText[0].line[0];
                    leftSubtitleUI.text = "";
                    rightSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.RIGHT):
                    rightSubtitleUI.text = currentText[0].line[0];
                    leftSubtitleUI.text = "";
                    centerSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.LEFT):
                    leftSubtitleUI.text = currentText[0].line[0];
                    centerSubtitleUI.text = "";
                    rightSubtitleUI.text = "";
                    break;
                case(SubtitleLocation.BOTH):
                        leftSubtitleUI.text = currentText[0].line[0];
                        rightSubtitleUI.text = currentText[0].line[1];
                        centerSubtitleUI.text = "";
                        break;
            }
            Invoke("Advance", currentText[0].voTime);
            currentText.RemoveAt(0);
        } else {
            Debug.Log("No text at this trigger");
        }
    }
    
}
