using System.Collections;
using UnityEngine;

[System.Serializable]
public class StreamLayout 
{
    [System.Serializable]
    public struct StreamData {
        public bool[] row;
    }

    public StreamData[] rows = new StreamData[7]; // Grid of 10 x 10

    public int sc = 7;
    public int bc = 4;

    private void OnValidate() {
        SetStreamDataSize(sc, bc);
    }

    public void SetStreamDataSize(int streamCount, int bulletCount) {
        sc = streamCount;
        bc = bulletCount;

        // set the length of rows to streamCount
        StreamData[] copyRows = new StreamData[sc];

        // set the length of row to bulletCount
        for (int i = 0; i < sc; i++) {
            copyRows[i].row = new bool[bc];
            for(int n = 0; n < bc; n++) {
                if(i < rows.Length && n < rows[i].row.Length) {
                    copyRows[i].row[n] = rows[i].row[n];
                } else {
                    copyRows[i].row[n] = false;
                }
            }
        }

        rows = copyRows;
    }
}
