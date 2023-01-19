using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCPosManager : MonoBehaviour
{
    public static MCPosManager Instance;
    
    public List<Vector3> positions=new List<Vector3>();
   

    private void Awake()
    {
        Instance = this;
        FightingManager.Instance.mcPosManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var children = transform.childCount;
        positions.Clear();
        for (int i = 0; i < children; i++)
        {
            positions.Add(transform.GetChild(i).transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPosByIndex(int index)
    {
        
        return positions[index];
    }

    public int GetIndexByPos(Vector3 pos)
    {
        float minDistance = 99999;
        var index = 0;
        for (int i = 0; i < positions.Count; i++)
        {
            var mark = positions[i];
            var tmpDis = Vector3.SqrMagnitude(mark - pos);
            if (tmpDis < minDistance)
            {
                minDistance = tmpDis;
                index = i;
            }
            
        }

        return index;
    }
}
