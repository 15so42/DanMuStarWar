using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCPosManager : MonoBehaviour
{
    public List<Vector3> positions=new List<Vector3>();
    public int firstPlanetPos=0;
    public int lastPlanetPos=9;
    
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
}
