using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnLoader : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bike;
    [SerializeField] private GameObject tagObj;
    [SerializeField] private List<SpawnPoint> spList;
    [SerializeField] private List<GameObject> vistas;
    [SerializeField] private LightingManager lightingManager;

    private int currentIndex = 1;
    public static SpawnLoader instance;

    public static SpawnLoader Instance {
        get{
            return instance;
        }
    }

    void Awake()
    {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        if(!lightingManager) {
            lightingManager = GetComponent<LightingManager>();
        }
    }

    public void LoadSpawnPoint(int spIndex) {
        Debug.Log("Hit SpawnLoader LoadSpawnPoint");
        if(spIndex != 0) {
            player.transform.position = spList[spIndex - 1].playerPoint;
            bike.transform.position = spList[spIndex - 1].bikePoint;
            if(vistas.Count > 0) {
                vistas[0].SetActive(false);
            }
            switch(spIndex){
                case 1: // coming out of meditation 1
                    currentIndex = 1;
                    lightingManager.SetTimeOfDay(11f);
                    vistas[1].SetActive(true);
                    break;
                case 2: // coming out of meditation 2
                    currentIndex = 2;
                    lightingManager.SetTimeOfDay(15f);
                    vistas[2].SetActive(true);
                    vistas[3].SetActive(true);
                    break;
                case 3: // coming out of meditation 3
                    currentIndex = 3;
                    lightingManager.SetTimeOfDay(4.5f);
                    break;
            }
        }
    }

    public int GetSpawnIndex()
    {
        return currentIndex;
    }
}
