using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarPlane : BattleUnit
{
   public List<Weapon> weapons=new List<Weapon>();

   protected void Start()
   {
      base.Start();
      foreach (var w in weapons)
      {
         w.Init(this);
      }
   }
   
   public void Attack()
   {
      foreach (var w in weapons)
      {
         w.Attack();
      }
      
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position,findEnemyDistance);
   }
}
