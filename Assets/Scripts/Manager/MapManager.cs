using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    
    private GameManager gameManager;
    private FightingManager fightingManager;
    
    [Header("地图范围")] public Vector3 start;
    public Vector3 end;
    
    [Header("装饰")]
    public int stoneNum;
    public GameObject[] stones;
    public float minStoneSize = 2;
    public float stoneSize = 10;

    [Header("星球预制体")] public GameObject[] planets;
    public float planetNum;
    public void Init(FightingManager fightingManager)
    {
        this.fightingManager = fightingManager;
    }

    public void PlaceAll()
    {
        SpawnStones();
        SpawnPlanets();
    }

    void SpawnPlanets()
    {
        for (int i = 0; i < planetNum; i++)
        {
            
        }
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
