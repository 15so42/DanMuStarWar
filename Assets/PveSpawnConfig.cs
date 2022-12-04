using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PveSpawnType
{
    Wait,
    Spawn,
}

[System.Serializable]
public struct PveSpawnStruct{
    public PveSpawnType type;
    public string values;
}

[CreateAssetMenu(fileName = "PVE生成配置")]
public class PveSpawnConfig : ScriptableObject
{

    
    public List<PveSpawnStruct> pveStructs=new List<PveSpawnStruct>();
    
}
