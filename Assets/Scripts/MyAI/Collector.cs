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
      foreach (var planet in GameManager.Instance.planetManager.allPlanets)
      {
         if(Random.Range(0,2)>0)
            continue;//为选择星球增加随机性
         if (planet.planetResContainer.HasAnyRes() && planet.owner==null && planet!=this.ownerPlanet)
         {
            ret = planet;
            break;
            
         }
      }

      return ret;
   }

   public void UpdateScanFx()
   {
      scanFx.transform.up = transform.position-minePlanet.transform.position;
   }

   public  override void OnAttacked(AttackInfo attackInfo)
   {
      base.OnAttacked(attackInfo);
      SkillManager.Instance.AddSkill("Skill_加速_LV1",this);
   }

 
   public void CollectEnd()
   {
      //获取资源
      for (int i = 0; i < canCollectRes.Count; i++)
      {
         canCollectRes[i].resourceNum+=collectSpeeds[i].speed;
      }
      
      //星球减少资源
      for (int j = 0; j < minePlanet.planetResContainer.allRes.Count; j++){
         
        minePlanet.planetResContainer.ReduceRes(minePlanet.planetResContainer.allRes[j].resourceType,collectSpeeds[j].speed);
      }
   }
   
   public void Deliver()
   {
      //获取资源
      foreach (var t in canCollectRes)
      {
         var resourceTable = t;
         ownerPlanet.planetResContainer.AddRes(resourceTable.resourceType,resourceTable.resourceNum);
         resourceTable.resourceNum=0;
      }
      
   }
   
   
}
