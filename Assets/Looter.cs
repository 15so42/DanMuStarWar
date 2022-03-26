using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;
using UnityTimer;

public class Looter : BattleUnit
{
   public int dicePoint;
   public int techPoint;
   
   public List<LooterPlane> looterPlanes=new List<LooterPlane>();
   private UnityTimer.Timer dieTimer;
   void Start()
   {
      base.Start();
      StartCoroutine(SpawnLooterPlaneC());
      dieTimer=Timer.Register(360, Die);
   }
   //舰载机
   void SpawnLooterPlane()
   {
      LooterPlane looterPlane = ResFactory.Instance.CreateBattleUnit(GameConst.BATTLE_UNIT_LOOTER_PLANE, transform.position).GetComponent<LooterPlane>();
      looterPlane.ownerLooter=this;
      looterPlanes.Add(looterPlane);
   }

   IEnumerator SpawnLooterPlaneC()
   {
      int count = 8;
      while (true)
      {
        
         if(count<0)
            yield break;
         count--;
         SpawnLooterPlane();
         yield return new WaitForSeconds(15);
      }
   }

   public override void Die()
   {
      base.Die();
      for (int i = 0; i < looterPlanes.Count; i++)
      {
         looterPlanes[i].Die();
      }
   }

   private void OnDisable()
   {
      dieTimer?.Cancel();
   }
}
