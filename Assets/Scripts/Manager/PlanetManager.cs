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

    public List<Planet> ownerAblePlanets=new List<Planet>();

    
    public int GetAlivePlanetCount()
    {
        int count=0;
        for (int i = 0; i < ownerAblePlanets.Count; i++)
        {
            if (ownerAblePlanets[i] != null && ownerAblePlanets[i].die == false)
            {
                count++;
            }
        }

        return count;
    }

    void OnPlanetCreated(Planet planet)
    {
        planet.SetIndex(allPlanets.Count);
        allPlanets.Add(planet);
        
       
        if (planet.canBeOwner)
        {
            ownerAblePlanets.Add(planet);
        }
    }

    void OnPlanetDie(Planet planet)
    {
        TipsDialog.ShowDialog(planet.owner+"的星球毁灭了",null);
        allPlanets.Remove(planet);
        ownerAblePlanets.Remove(planet);
        if (allPlanets.Count == 1)
        {
            FightingManager.Instance.GameOver(planet);
            allPlanets[0].Die();
            allPlanets.Clear();
        }

       
    }
    
    

    /// <summary>
    /// 管理员测试用工具
    /// </summary>
    public void BattleOverByAdmin()
    {
        for (int i = 0; i < allPlanets.Count - 1; i++)
        {
            allPlanets[i].OnAttacked(new AttackInfo(null,AttackType.Real,1000));
        }
    }
}
