using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public ExternalController player;
    [SerializeField]
    private GameObject originalCamera;

    private bool wasCutsceneTriggered = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player" && !wasCutsceneTriggered)
        {
            StartCutscene();

            StartCoroutine(Test());
        }
    }

    public void StartCutscene()
    {
        Debug.Log("Start Cutscene");
        player.SetPaused(true);
        wasCutsceneTriggered = true;
        originalCamera.SetActive(false);
    }

    public void EndCutscene()
    {
        player.SetPaused(false);
        originalCamera.SetActive(true);
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(5);
        EndCutscene();
    }
}
