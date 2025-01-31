using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntryActivationTestScript : MonoBehaviour

{
    public EntryController targetJournalEntry;
    public bool unlockEntry = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (unlockEntry)
        {
            targetJournalEntry.ActivateEntry();
            unlockEntry = false;
        }

    }
}
