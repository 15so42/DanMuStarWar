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
    private List<string> plantesName=new List<string>();
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
    }

    public void PlaceAll()
    {
        SpawnStones();
        SpawnPlanets();
    }

    void SpawnPlanets()
    {
        var empty=new GameObject("planets");
        for (int i = 0; i < planetNum; i++)
        {
            var pfb = planets[Random.Range(0, planets.Length)];
            Vector3 randomPos=new Vector3(Random.Range(start.x,end.x),0,Random.Range(start.z,end.z));
            var go = GameObject.Instantiate(pfb, randomPos, Quaternion.identity, empty.transform);
            Debug.Log("aaaaaaaaaa");
            go.GetComponent<Planet>().SetUpPlanet(plantesName[Random.Range(0,plantesName.Count)]);
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
