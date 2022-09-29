using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitherHeadBullet : ArrowBullet
{
   
   protected override void DamageVictim(IVictimAble victim)
   {
      AttackManager.Instance.Explosion(new AttackInfo(owner,AttackType.Physics,handWeapon.attackValue*2), handWeapon,victim.GetVictimPosition(),15);
      var skill=SkillManager.Instance.AddSkill("Skill_凋零_LV1", victim.GetVictimEntity(), handWeapon.owner.planetCommander);
      if (skill as WitherSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
      {
         (skill as WitherSkill).SetAttacker(handWeapon.owner);

         (skill as WitherSkill).damage = Mathf.CeilToInt (handWeapon.attackValue / 5f);
      }
      
      base.DamageVictim(victim);
      
   }
   
   
}
