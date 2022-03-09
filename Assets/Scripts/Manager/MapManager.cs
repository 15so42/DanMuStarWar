using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    
    private GameManager gameManager;
    private FightingManager fightingManager;
    
    [Header("地图范围")] public Vector3 start;
    public Vector3 end;
    public float planetRadius = 80;
    public int xSize = 1;
    public int zSize = 1;
    public int xNum = 10;
    public int zNum = 6;
    public float heightRange = 20;
    public string[,] grids;//每个格子存放对应的星球，只用来生成，不用太在意
    [Header("星球最少间隔多少格子单位")]
    public int safeRadius = 1;
    
    [Header("陨石地图范围")] public float stoneRadius=80;
    
    
    [Header("装饰")]
    public int stoneNum;
    public GameObject[] stones;
    public float minStoneSize = 2;
    public float stoneSize = 10;

    [Header("星球预制体")] public GameObject[] planets;
    public float planetNum;
    private List<string> plantesName=new List<string>();
    
    //分格子放星球
    
    public void Init(FightingManager fightingManager)
    {
        this.fightingManager = fightingManager;
        //这里不知道怎么写了，只能这样手动加了
        plantesName.Add(GameConst.EARTH);
        plantesName.Add(GameConst.EARTH_LIKE);
        plantesName.Add(GameConst.GRAY_PLANET);
        plantesName.Add(GameConst.MINE_PLANET);
        plantesName.Add(GameConst.VIRTUAL_PLANET);
        plantesName.Add(GameConst.TECH_PLANET);
        plantesName.Add(GameConst.MC_PLANET);
        plantesName.Add(GameConst.BIOLOGY_PLANET);
        
        grids=new string[zNum,xNum];
    }

    public void PlaceAll()
    {
        SpawnStones();
        SpawnPlanets();
    }

    Vector3 GetWorldPosByGridPos(int x,int y)
    {
        return start + Vector3.right * y * xSize + Vector3.forward * x * zSize;
    }

    bool HasNearPlanet(int x, int y,int radius)//传入数据坐标
    {
        //下面用数据坐标
        var hasNear = false;
        for (int i = x - radius; i <= x + radius; i++)
        {
            for (int j = y - radius; j <= y + radius; j++)
            {
                var newi = Mathf.Clamp(i, 0, zNum-1);
                var newj= Mathf.Clamp(j,0,xNum-1);
                
                if (!string.IsNullOrEmpty(grids[newi, newj]))
                {
                    hasNear = true;
                }
            }
        }

        
        return hasNear;
    }

    void SpawnPlanets()
    {
        var empty=new GameObject("planets");
        for (int i = 0; i < planetNum; i++)
        {
            var pfb = planets[Random.Range(0, planets.Length)];
            Vector2Int gridPos=new Vector2Int(Random.Range(0,zNum),Random.Range(0,xNum));
            int retryCount = 10000;
            while ((String.IsNullOrEmpty(grids[gridPos.x, gridPos.y]) == false || HasNearPlanet(gridPos.x,gridPos.y,safeRadius)) && retryCount>0)
            {
                gridPos=new Vector2Int(Random.Range(0,zNum),Random.Range(0,xNum));
                retryCount--;
            }//已经有星球了

            if (retryCount == 0)
            {
                Debug.Log("没有足够空间:"+retryCount);
                continue;
            }

            var worldPos = GetWorldPosByGridPos(gridPos.x,gridPos.y);
            worldPos.y = Random.Range(-1 * heightRange, heightRange);
            
            
            var go = GameObject.Instantiate(pfb, worldPos, Quaternion.identity, empty.transform);
            var planetName = plantesName[Random.Range(0, plantesName.Count)];
            go.GetComponent<Planet>().SetUpPlanet(planetName);
            grids[gridPos.x, gridPos.y] = planetName;

        }
    }

    void SpawnStones()
    {
        var empty=new GameObject("stones");
        for (int i = 0; i < stoneNum; i++)
        {
            Vector3 randomPos=Random.insideUnitSphere*stoneRadius;
            var stone=GameObject.Instantiate(stones[Random.Range(0, stones.Length)], randomPos, Quaternion.LookRotation(randomPos.normalized), empty.transform);
            stone.transform.localScale=Vector3.one*minStoneSize+Random.insideUnitSphere*stoneSize;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color=Color.green;
        Gizmos.DrawWireCube((start+end)/2,new Vector3((end.x-start.x),heightRange,(end.z-start.z)));
        for (int i = 0; i < zNum; i++)
        {
            for (int j = 0; j < xNum; j++)
            {
                Gizmos.DrawWireCube(GetWorldPosByGridPos(i,j),Vector3.one);
            }
        }
    }
}
