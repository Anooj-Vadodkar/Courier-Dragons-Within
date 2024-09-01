using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtNarrativeHolder : MonoBehaviour
{
    [SerializeField] private List<NarrativeCollisionScript> narrativeList;

    public void PlayNext() {
        if(narrativeList.Count > 0) {
            narrativeList[0].OnBreathe();
            narrativeList.RemoveAt(0);
        }
    }
}
