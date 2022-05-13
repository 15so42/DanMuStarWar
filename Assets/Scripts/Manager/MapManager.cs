using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ludiq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    
    private GameManager gameManager;
    private FightingManager fightingManager;
    
    [Header("地图范围")] public Vector3 start;
    public Vector3 end;
    
    public int xSize = 1;
    public int zSize = 1;
    public int xNum = 10;
    public int zNum = 6;
    public float heightRange = 20;
    public string[,] grids;//每个格子存放对应的星球，只用来生成，不用太在意
    [Header("星球最少间隔多少格子单位")]
    public int safeRadius = 1;

    public Vector3 stoneCenter = Vector3.zero;
    [Header("陨石地图范围")] public float stoneRadius=80;
    
    
    [Header("装饰")]
    public int stoneNum;
    public GameObject[] stones;
    public float minStoneSize = 2;
    public float stoneSize = 10;

    public GameObject[] planets;//预制体
    [Header("可居住星球预制体")] public string[] playerPlanets;
    [Header("Res星球预制体")] public string[] resPlanets;
    public int playerPlanetNum=6;
    public int resPlanetNum = 3;
    public ColorTable colorTable;


    [Header("存放位置")] public Transform planetRoot;
    //public Transform battleUnitRoot;
   
    //固定位置地图
    [Header("团战模式地图")]
    public List<MapPosData> mapPoses=new List<MapPosData>();
    [Header("混战模式地图")]
    public List<MapPosData> normalMapPoses=new List<MapPosData>();
    
    //分格子放星球
    
    public void Init(FightingManager fightingManager)
    {
        
        this.fightingManager = fightingManager;
        
        
        grids=new string[zNum,xNum];
        if(fightingManager.gameMode!=GameMode.MCWar)
            SpawnStones();
    }

    public void PlaceAll()
    {
        
        SpawnAllPlanets();
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

    void SpawnAllPlanets()
    {
        if (fightingManager.gameMode == GameMode.Normal)
        {
            //SpawnPlanets(playerPlanets,playerPlanetNum,false,fightingManager.gameMode);
            //SpawnPlanets(resPlanets,resPlanetNum,true,fightingManager.gameMode);
            SpawnPlantesByMapPos(GameMode.Normal);
        }
        else
        {
            if(fightingManager.gameMode == GameMode.BattleGround)
                SpawnPlantesByMapPos(GameMode.BattleGround);
            else
            {
                SpawnPlantesByMapPos(GameMode.MCWar);
            }
        }
        
        EventCenter.Broadcast(EnumEventType.OnPlanetsSpawned);
    }

    void SpawnPlantesByMapPos(GameMode gameMode)
    {
        var empty=new GameObject("planets");
        empty.transform.SetParent(planetRoot);
        //empty.transform.position=new Vector3(0,48.86f,-2.55f);

        var realMapFile = this.mapPoses;
        if (gameMode == GameMode.Normal)
            realMapFile = this.normalMapPoses;
            
        var planetNum = realMapFile[0].posData.Count;
        var pfb = planets[Random.Range(0, planets.Length)];
        
        for (int i = 0; i < planetNum; i++)
        {
            var worldPos = realMapFile[0].posData[i];
            if (gameMode == GameMode.BattleGround || gameMode==GameMode.Normal)
            {
                worldPos+=new Vector3(0,48.86f,-2.55f);//团战模式相机会近一些
            }
            
            
            var planetsName = playerPlanets.Concat(resPlanets).ToList();
        
        
            var planetName = planetsName[Random.Range(0, planetsName.Count)];
            //特定位置使用玩家星球
            if (i == 0 || i == planetNum - 1 && gameMode==GameMode.BattleGround)
            {
                planetName = playerPlanets[Random.Range(0, playerPlanets.Length)];
            }

            if (i < fightingManager.maxPlayerCount && gameMode == GameMode.Normal)
            {
                planetName = playerPlanets[Random.Range(0, playerPlanets.Length)];
            }
            
            
            var go = GameObject.Instantiate(pfb, worldPos, Quaternion.identity, empty.transform);

            if (gameMode == GameMode.BattleGround)
            {
                var color = i > 5 ?  i>11 ?Color.red : Color.cyan  : Color.yellow;
            
                go.GetComponent<Planet>().SetUpPlanet(planetName,color);
            }
            else
            {
                go.GetComponent<Planet>().SetUpPlanet(planetName,colorTable.colors[i]);
            }
           
                
        }

        
        
    }

    void SpawnPlanets(string[] planetsName,int planetNum,bool resPlanet,GameMode gameMode)
    {
        var empty=new GameObject("planets");
        empty.transform.SetParent(planetRoot);

        
        
      
        for (int i = 0; i < planetNum; i++)
        {
            var pfb = planets[Random.Range(0, planets.Length)];
            //Vector2Int gridPos=new Vector2Int(Random.Range(0,zNum),Random.Range(0,xNum));
            Vector2Int gridPos=Vector2Int.zero;
            // int retryCount = 10000;
            //随机
           /* while ((String.IsNullOrEmpty(grids[gridPos.x, gridPos.y]) == false || HasNearPlanet(gridPos.x,gridPos.y,safeRadius)) && retryCount>0)
            {
                gridPos=new Vector2Int(Random.Range(0,zNum),Random.Range(0,xNum));
                retryCount--;
            }//已经有星球了

            if (retryCount == 0)
            {
                Debug.Log("没有足够空间:"+retryCount);
                continue;
            }*/
           
           var worldPos =Vector3.zero;

          
               //圆形地图
               if (resPlanet)
               {
                   if (i < 8)
                   {
                       worldPos = Vector3.zero + Vector3.right * (1.3f * (Mathf.Sin(  45+Mathf.Deg2Rad*(i+1)*360/resPlanetNum) * 45)) + Vector3.forward *  1.4f*(Mathf.Cos(45+Mathf.Deg2Rad*(i+1)*360/resPlanetNum) * 35);
                   }
                   else
                   {
                       //worldPos = Vector3.zero + Vector3.right * (Mathf.Sin(  Mathf.Deg2Rad*(i+1)*90) * 120) + Vector3.forward * (Mathf.Cos(Mathf.Deg2Rad*(i+1)*90) * 120);
                   }
                
               }
               else
               {
                   worldPos = Vector3.zero + Vector3.right * (Mathf.Sin(Mathf.Deg2Rad*(i+1)*360/playerPlanetNum) * 80)*1.4f + Vector3.forward * (Mathf.Cos(Mathf.Deg2Rad*(i+1)*360/playerPlanetNum) * 80);
               }
           


            var planetName = planetsName[Random.Range(0, planetsName.Length)];
            var go = GameObject.Instantiate(pfb, worldPos, Quaternion.identity, empty.transform);
            go.GetComponent<Planet>().SetUpPlanet(planetName,colorTable.colors[resPlanet?playerPlanetNum+i:i]);
            grids[gridPos.x, gridPos.y] = planetName;

        }
    }

    void SpawnStones()
    {
        var empty=new GameObject("stones");
        for (int i = 0; i < stoneNum; i++)
        {
            Vector3 randomPos=Random.insideUnitSphere*stoneRadius+stoneCenter;
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
