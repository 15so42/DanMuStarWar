using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wither : Zombie
{
   private float timer = 0;
   protected override void Update()
   {
      base.Update();
      timer += Time.deltaTime;
      if (timer > 4)
      {
         OnAttacked(new AttackInfo(this, AttackType.Heal, (int) (props.maxHp * 0.01f)));
         timer = 0;
      }

   }

   protected override void RemoveSpell(HandWeapon handWeapon)
   {
      base.RemoveSpell(handWeapon);
      handWeapon.randomStrs.Remove("烈阳");
   }
}
