using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class CollectSpeed
{
   public ResourceType resourceType;
   public int speed;
}
public class Collector : BattleUnit
{
   public Planet minePlanet;//采矿目标星球

   public GameObject scanFx;

   [Header("收集资源，两个配置表资源顺序要一样")] 
   public List<ResourceTable> canCollectRes=new List<ResourceTable>();

   public List<CollectSpeed> collectSpeeds = new List<CollectSpeed>();
   
   public Planet FindAnyResPlanet()
   {
      Planet ret = null;
      
      var planets=ownerPlanet.colonyPlanets;

      if (planets.Count == 0)
         return null;//没有殖民地
      var planet = planets[UnityEngine.Random.Range(0, planets.Count)];
      
      for (int i = 0; i < ownerPlanet.allyPlanets.Count; i++)
      {
         if (ownerPlanet.allyPlanets[i].colonyPlanets.Contains(planet))
         {
            ret = planet;
            return ret;//如果这颗星球是友军的，直接返回
         }
      }
      if (Math.Abs(planet.colonyPoint - planet.needRingPoint) < 1 &&  planet.planetResContainer.HasAnyRes() && (planet.owner == null || planet.owner.die) && planet != this.ownerPlanet && ownerPlanet.colonyPlanets.Contains(planet))
      {
         ret = planet;
      }
      

      return ret;
   }

   public void UpdateScanFx()
   {
      scanFx.transform.up = transform.position-minePlanet.transform.position;
   }

  

 
   public void CollectEnd()
   {
      //获取资源
      for (int i = 0; i < canCollectRes.Count; i++)
      {
         if (minePlanet.planetResContainer.GetResNumByType(canCollectRes[i].resourceType) > 0)
         {
            canCollectRes[i].resourceNum+=collectSpeeds[i].speed;
            minePlanet.planetResContainer.ReduceRes(canCollectRes[i].resourceType,collectSpeeds[i].speed);
         }
         
      }
      
     
   }

   public override void Die()
   {
      base.Die();
      StopAllCoroutines();
   }

   public void Deliver()
   {
      var tipStr = "";
      //获取资源
      foreach (var t in canCollectRes)
      {
         var resourceTable = t;
         ownerPlanet.planetResContainer.AddRes(resourceTable.resourceType,resourceTable.resourceNum);

         if (resourceTable.resourceType == ResourceType.DicePoint)
         {
            
            tipStr+="骰子 +"+resourceTable.resourceNum+"  ";
            if (resourceTable.resourceNum > 0)
            {
               planetCommander.AddPoint(1);
            }
         }
         if(resourceTable.resourceType==ResourceType.Tech)
            tipStr+="科技 +"+resourceTable.resourceNum+"  ";
         
         resourceTable.resourceNum=0;
         
      }
      
      if(ownerPlanet)
         ownerPlanet.LogTip(tipStr);
      
   }
   
   
}
