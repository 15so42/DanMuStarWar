using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : BattleUnit
{
   public Planet minePlanet;//采矿目标星球

   public override Planet GetPlanet()
   {
      return base.GetPlanet();
   }

   public Planet FindAnyResPlanet()
   {
      Planet ret = null;
      foreach (var planet in GameManager.Instance.planetManager.allPlanets)
      {
         if (planet.planetResMgr.HasAnyRes() && planet.owner==null && planet!=this.ownerPlanet)
         {
            ret = planet;
            break;
            
         }
      }

      return ret;
   }
}
