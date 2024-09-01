using UnityEngine;

[CreateAssetMenu(fileName = "BossScriptableObject", menuName = "ScriptableObjects/BossData")]
public class BossScriptableObject : ScriptableObject
{
    public int sections = 4;
    public int streamCount = 4;
    int streamLength = 10;
    public float bulletFreq = 0.5f;
}
