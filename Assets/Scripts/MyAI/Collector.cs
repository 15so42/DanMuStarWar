using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collector : BattleUnit
{
   public Planet minePlanet;//采矿目标星球

   public GameObject scanFx;
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
}
