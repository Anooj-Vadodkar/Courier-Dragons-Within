using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
// using UnityEngine.Rendering;
using AYellowpaper.SerializedCollections;
//using UnityEditor.Build.Content;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private bool inMeditation = false;

    [SerializeField] private EventReference mediMusic;
    [SerializeField] private EventReference natureAmbient;
    [SerializedDictionary("Name", "Color")] public SerializedDictionary<string, Color> characters;
    private EventInstance natureAmbientInstnace;
    private AudioManager audioManager;
    [SerializeField] private int currentMeditation;

    public static GameManager Instance {
        get {
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(_instance != null && _instance != this) {
            GameManager._instance.SetisInMeditiation(inMeditation);
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += LevelLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= LevelLoaded;
    }

    private void Start() {
        if(inMeditation) {
            audioManager = AudioManager.Instance;
            // audioManager.PlayEvent(FMODEvents.Instance.natureAmbient, Camera.main.transform.position);
        }

        /* if(SpawnLoader.Instance) {
            SpawnLoader.Instance.LoadSpawnPoint(currentMeditation);
        } */
    }

    private void Update() {
        if(InputManager.Instance.GetBikingConvoScenePressed()) {
            audioManager.StopMusic();
            SceneManager.LoadScene("Day1Scene_MWHVersion");
        }

        if(InputManager.Instance.GetMediScenePressed()) {
            SceneManager.LoadScene("Meditation1");
        }

        if(InputManager.Instance.GetReloadPressed()) {
            SceneManager.LoadScene("MainMenu");
        }

        if(InputManager.Instance.GetQuitGamePressed()) {
            Application.Quit();
        }
    }

    private void LevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name.Equals("Day1Scene_MWHVersion")) {
            Debug.Log("Hit level load with meditation: " + currentMeditation);
            if(SpawnLoader.Instance) {
                SpawnLoader.Instance.LoadSpawnPoint(currentMeditation);
            }
            // Get necessary objects to move
                // player
                // bike
                // tag
                // invisible walls

            // Spawn each object in the ocrrect location

            // Play correct cutscene
        }
    }

    /* private void LevelLoaded() {
        if(SceneManager.GetActiveScene().name.Equals("Day1Scene_MHWVersion")) {
            Debug.Log("Hit level load with meditation: " + currentMeditation);
            if(SpawnLoader.Instance) {
                SpawnLoader.Instance.LoadSpawnPoint(currentMedit)
            }
        }
    } */

    public void SetMeditationNum(int num) {
        currentMeditation = num;
    }

    public void SceneTranstition(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public bool isInMeditation() {
        return inMeditation;
    }

    public void SetisInMeditiation(bool inMeditation) {
        this.inMeditation = inMeditation;
    }
}
