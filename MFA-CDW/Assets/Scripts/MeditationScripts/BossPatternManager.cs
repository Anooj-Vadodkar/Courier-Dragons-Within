using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BossPatternManager : MonoBehaviour
{
    public StreamLayout[] data = new StreamLayout[4];

    private void OnValidate() {
        foreach(StreamLayout s in data) {
            if(s != null) {
                s.SetStreamDataSize(s.sc,s.bc);
            }
        }
    }
}
