using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("地图范围")] public Vector3 start;
    public Vector3 end;
    
    
    
    public int stoneNum;
    public GameObject[] stones;
    public float minStoneSize = 2;
    public float stoneSize = 10;
    public void Init()
    {
        SpawnStones();
    }

    void SpawnStones()
    {
        var empty=new GameObject("stones");
        for (int i = 0; i < stoneNum; i++)
        {
            Vector3 randomPos=new Vector3(Random.Range(start.x,end.x),0,Random.Range(start.z,end.z));
            var stone=GameObject.Instantiate(stones[Random.Range(0, stones.Length)], randomPos, Quaternion.LookRotation(randomPos.normalized), empty.transform);
            stone.transform.localScale=Vector3.one*minStoneSize+Random.insideUnitSphere*stoneSize;
        }
    }
}
