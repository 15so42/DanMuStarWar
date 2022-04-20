using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEditor;
using UnityEngine;

public class MapPosRecorder : MonoBehaviour
{
    public MapPosData mapPosData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Record();
        }
    }
    public void Record()
    {
        var children = transform.childCount;
        mapPosData.posData.Clear();
        for (int i = 0; i < children; i++)
        {
            mapPosData.posData.Add(transform.GetChild(i).transform.position);
        }
    }
        
        
    
}
