using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarPlane : BattleUnit
{
   public List<Weapon> weapons=new List<Weapon>();

   
   protected void Start()
   {
      base.Start();
      weapons = GetComponentsInChildren<Weapon>().ToList();
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
   
   
   
   public  override void OnAttacked(AttackInfo attackInfo)
   {
      base.OnAttacked(attackInfo);
      //if(!chaseTarget)
         SetChaseTarget(attackInfo.attacker as BattleUnit);
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position,findEnemyDistance);
   }
}
