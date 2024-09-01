using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class SubtitleScroller : MonoBehaviour
{
    [SerializeField] private GameObject subtitlePrefab;
    [SerializeField] private Transform leftSubtitleOrigin;
    [SerializeField] private Transform rightSubtitleOrigin;
    [SerializeField] private TextMeshProUGUI centerSubtitleContent;
    [SerializeField] private TextMeshProUGUI centerSubtitleName;
    [SerializeField] private float scrollAmt = 200;

    private List<GameObject> subtitlesInstances;

    private List<NewSub> currentSubtitles;

    [System.Serializable]
    public class NewSub {
        public List<NewLine> lines = new List<NewLine>{};
        public float voTime;
        public SubtitleLocation location;
        public List<EventReference> oneShotAudio;
        public UnityEvent eventRef;
    }

    public enum SubtitleLocation {
        LEFT,
        RIGHT,
        CENTER,
        BOTH
    }

    [System.Serializable]
    public class NewLine {
        public string content;
        public string name;
    }

    private void Start() {
        subtitlesInstances = new List<GameObject>();
    }

    private void Advance() {
        // Instantiate a new subtitle instance 
        if(currentSubtitles.Count > 0) {
            foreach(GameObject instance in subtitlesInstances) {
                if(instance) {
                    // Start coroutine that
                    // move it up
                    StartCoroutine(MoveUp(instance));
                    // decrease alpha
                    StartCoroutine(DecreaseAlpha(instance));
                }
            }
            GameObject leftInstance, rightInstance;
            switch(currentSubtitles[0].location) {
                case SubtitleLocation.LEFT:
                    leftInstance = Instantiate(subtitlePrefab, leftSubtitleOrigin);
                    rightInstance = Instantiate(subtitlePrefab, rightSubtitleOrigin);
                    leftInstance.GetComponent<NewLineContainer>().contentText.text = currentSubtitles[0].lines[0].content;
                    leftInstance.GetComponent<NewLineContainer>().nameText.text = currentSubtitles[0].lines[0].name;
                    leftInstance.GetComponent<NewLineContainer>().nameText.color = CheckName(currentSubtitles[0].lines[0].name);
                    rightInstance.GetComponent<NewLineContainer>().contentText.text = "";
                    rightInstance.GetComponent<NewLineContainer>().nameText.text = "";
                    subtitlesInstances.Add(leftInstance);
                    subtitlesInstances.Add(rightInstance);
                    ClearCenter();
                    break;
                case SubtitleLocation.RIGHT:
                    leftInstance = Instantiate(subtitlePrefab, leftSubtitleOrigin);
                    rightInstance = Instantiate(subtitlePrefab, rightSubtitleOrigin);
                    rightInstance.GetComponent<NewLineContainer>().contentText.text = currentSubtitles[0].lines[0].content;
                    rightInstance.GetComponent<NewLineContainer>().nameText.text = currentSubtitles[0].lines[0].name;
                    rightInstance.GetComponent<NewLineContainer>().nameText.color = CheckName(currentSubtitles[0].lines[0].name);
                    leftInstance.GetComponent<NewLineContainer>().contentText.text = "";
                    leftInstance.GetComponent<NewLineContainer>().nameText.text = "";
                    subtitlesInstances.Add(leftInstance);
                    subtitlesInstances.Add(rightInstance);
                    ClearCenter();
                    break;
                case SubtitleLocation.CENTER:
                    centerSubtitleContent.text = currentSubtitles[0].lines[0].content;
                    centerSubtitleName.text = currentSubtitles[0].lines[0].name;
                    centerSubtitleName.color = CheckName(currentSubtitles[0].lines[0].name);
                    break;
                case SubtitleLocation.BOTH:
                    leftInstance = Instantiate(subtitlePrefab, leftSubtitleOrigin);
                    rightInstance = Instantiate(subtitlePrefab, rightSubtitleOrigin);
                    leftInstance.GetComponent<NewLineContainer>().contentText.text = currentSubtitles[0].lines[0].content;
                    leftInstance.GetComponent<NewLineContainer>().nameText.text = currentSubtitles[0].lines[0].name;
                    leftInstance.GetComponent<NewLineContainer>().nameText.color = CheckName(currentSubtitles[0].lines[0].name);
                    rightInstance.GetComponent<NewLineContainer>().contentText.text = currentSubtitles[0].lines[1].content;
                    rightInstance.GetComponent<NewLineContainer>().nameText.text = currentSubtitles[0].lines[1].name;
                    rightInstance.GetComponent<NewLineContainer>().nameText.color = CheckName(currentSubtitles[0].lines[1].name);
                    subtitlesInstances.Add(leftInstance);
                    subtitlesInstances.Add(rightInstance);
                    ClearCenter();
                    break;
            }

            // plays all the audio associated with this subtitle
            if(currentSubtitles[0].oneShotAudio.Count > 0) {
                foreach(EventReference audioReference in currentSubtitles[0].oneShotAudio) {
                    AudioManager.Instance.PlayEvent(audioReference);
                }
            }

            currentSubtitles[0].eventRef.Invoke();

            Invoke("Advance", currentSubtitles[0].voTime);
            currentSubtitles.RemoveAt(0);
        } else {
            Debug.Log("No More Subtitles");
            Clear();
        }
    }

    private IEnumerator MoveUp(GameObject instance) {
        Vector3 targetPos = instance.transform.position + new Vector3(0, scrollAmt, 0);
        float lerp = 0;
        while(lerp < 1 && instance) {
            lerp += Time.deltaTime;
            if(lerp >= 1) {
                lerp = 1;
            }
            instance.transform.position = Vector3.Lerp(instance.transform.position, targetPos, lerp);
            yield return null;
        }
    }

    private IEnumerator DecreaseAlpha(GameObject instance) {
        NewLineContainer instanceContainer = instance.GetComponent<NewLineContainer>();
        Color contentStartingColor = instanceContainer.contentText.color;
        Color nameStartingColor = instanceContainer.nameText.color;
        float decreaseAmt = 0;
        while(decreaseAmt < (1.0f / 3.0f)) {
            decreaseAmt += Time.deltaTime / 3.0f;
            if(decreaseAmt >= (1.0f / 3.0f)) {
                decreaseAmt = (1.0f / 3.0f);
            }
            instanceContainer.contentText.color = new Color(contentStartingColor.r, contentStartingColor.g, contentStartingColor.b, contentStartingColor.a - decreaseAmt);
            instanceContainer.nameText.color = new Color(nameStartingColor.r, nameStartingColor.g, nameStartingColor.b, nameStartingColor.a - decreaseAmt);
            yield return null;
        }

        if(instanceContainer.contentText.color.a < 0.2f) {
            subtitlesInstances.Remove(instance);
            Destroy(instance);
        }
    }

    private void Clear() {
        foreach(GameObject instance in subtitlesInstances) {
            Destroy(instance);
        }
        centerSubtitleContent.text = "";
        centerSubtitleName.text = "";
    }

    private void ClearCenter() {
        centerSubtitleContent.text = "";
        centerSubtitleName.text = "";
    }

    public void InitializeSubtitleBlock(NewSub[] subtitles) {
        currentSubtitles = subtitles.ToList<NewSub>();
        if(currentSubtitles.Count > 0) {
            Advance();
        }
    }

    private Color CheckName(string name) {
        if(GameManager.Instance.characters.ContainsKey(name)) {
            return GameManager.Instance.characters[name];
        } else {
            return Color.black;
        }
    }
}
