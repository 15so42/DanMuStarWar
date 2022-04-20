using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "地图位置数据")]
public class MapPosData : ScriptableObject
{
    
   public List<Vector3> posData=new List<Vector3>();
}
