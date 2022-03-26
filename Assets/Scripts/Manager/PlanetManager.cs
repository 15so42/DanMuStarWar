using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance;

    void Awake()
    {
        Instance = this;
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetDie,OnPlanetDie);
    }
    public List<Planet> allPlanets=new List<Planet>();

    //public List<Planet> ownerAblePlanets=new List<Planet>();

    
    

    void OnPlanetCreated(Planet planet)
    {
        planet.SetIndex(allPlanets.Count);
        allPlanets.Add(planet);


        // if (planet.canBeOwner)
        // {
        //     ownerAblePlanets.Add(planet);
        // }
    }

    void OnPlanetDie(Planet planet)
    {
        TipsDialog.ShowDialog(planet.owner==null?"无人星球毁灭了": planet.owner.userName + "的星球毁灭了",null);
        //allPlanets.Remove(planet);
       
        
        //获得剩余玩家数
        var count = 0;
        Planet lastAlivePlanet = null;//最后存活的星球
        
        for (int i = 0; i < allPlanets.Count; i++)
        {
            if (allPlanets[i].die == false && allPlanets[i].owner!=null)//存活并且有玩家
            {
                count++;
                lastAlivePlanet = allPlanets[i];
            }
        }
        
       
        if (count == 1)//如果存活的星球只剩最后一个
        {
            FightingManager.Instance.GameOver(lastAlivePlanet);
            //清除剩下所有的星球
            
            for (int i = 0; i < allPlanets.Count; i++)
            {
                
                if(allPlanets[i].die)//已经死过的不用再死了
                    continue;
                allPlanets[i].Die();
                i--;


            }
            allPlanets.Clear();

        }

       
    }
    
    

    /// <summary>
    /// 管理员测试用工具
    /// </summary>
    public void BattleOverByAdmin()
    {
        for (int i = 0; i < allPlanets.Count ; i++)
        {
            allPlanets[i].OnAttacked(new AttackInfo(null,AttackType.Real,5000));
            i--;
        }
    }
}
