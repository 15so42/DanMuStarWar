﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarPlane : BattleUnit,ISupportAble
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
      var attackerOwner = attacker.GetAttackerOwner();
      var victimOwner = GetVictimOwner();
      var aName=attacker.GetAttackEntity().gameObject;
      var vName=gameObject.name;
      
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
