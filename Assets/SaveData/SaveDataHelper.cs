using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SaveDataHelper")]
public class SaveDataHelper : ScriptableObject
{
    [Header("DanMuReceiver")] public long lastReadUnix;
    
}
