using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum WarPlaneType
{
   None,//default选项
   WarPlane,
   GuardPlane,
}
public class WarPlane : BattleUnit,ISupportAble
{
   public List<Weapon> weapons=new List<Weapon>();

   public WarPlaneType warPlaneType=WarPlaneType.None;
   protected virtual void Start()
   {
      base.Start();
      weapons = GetComponentsInChildren<Weapon>().ToList();
      foreach (var w in weapons)
      {
         w.Init(this);
      }
   }
   
   public override void Attack()
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
      var attacker = attackInfo.attacker;
      if(attacker==null)//attacker为null表示是系统，或者事件导致的扣血
         return;
      var attackerOwner = attacker.GetAttackerOwner();
      var victimOwner = GetVictimOwner();
      
      
      if ( attackerOwner != victimOwner) 
      {
         var victim = attacker.GetAttackEntity();
         SetChaseTarget(victim);
      }
         
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position,findEnemyDistance);
   }

   public void Support(BattleUnit attacker)
   {
      
      SetChaseTarget(attacker);
   }

  
   
}
