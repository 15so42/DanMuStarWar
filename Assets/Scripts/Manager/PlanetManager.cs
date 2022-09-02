using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private void OnDestroy()
    {
        EventCenter.RemoveListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
        EventCenter.RemoveListener<Planet>(EnumEventType.OnPlanetDie,OnPlanetDie);
    }

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
        Planet lastAlivePlanet = null;//遍历中最后存活的星球
        
        for (int i = 0; i < allPlanets.Count; i++)
        {
            if (allPlanets[i].die == false && allPlanets[i].owner!=null)//存活并且有玩家
            {
                count++;
                lastAlivePlanet = allPlanets[i];
            }
        }
        
       
        if (count == 1 || count==0)//如果存活的星球只剩最后一个
        {
            if(FightingManager.Instance.gameStatus==GameStatus.WaitingNewFighting)
                return;
            
            if (FightingManager.Instance.gameMode==GameMode.MCWar|| FightingManager.Instance.gameMode==GameMode.Marble)
            {
                List<PlanetCommander> losers = new List<PlanetCommander>();
                List<PlanetCommander> winners = null;
                
                for (int i = 0; i < allPlanets.Count; i++)
                {
                    if (allPlanets[i].die)
                    {
                        losers=losers.Concat(allPlanets[i].planetCommanders).ToList();
                    }
                    else
                    {
                        winners = allPlanets[i].planetCommanders;
                    }
                }

                var upload = true;
                if (FightingManager.Instance.roundManager as McPveRoundManager)
                    upload = false;
                //PVE模式不上传胜场和败场数据，其他照传不误
                FightingManager.Instance.GameOverByMc(winners,losers,upload);
            }
            else
            {
                FightingManager.Instance.GameOver(lastAlivePlanet,FightingManager.Instance.roundManager.desireMode);
            }
            
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
            if (!allPlanets[i].die)
            {
                allPlanets[i].OnAttacked(new AttackInfo(null,AttackType.Real,5000));
                i--;
            }
            
        }
    }
}
