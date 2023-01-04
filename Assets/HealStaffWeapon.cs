using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;

public class HealStaffWeapon : HandWeapon
{
    public override AttackInfo GetBaseAttackInfo()
    {
        return new AttackInfo(owner,AttackType.Real,attackValue+GetWeaponLevelByNbt("治疗"));
    }

    public override float GetAttackSpeed()
    {
        var attackSpeedLevel = GetWeaponLevelByNbt("妙手");
        return attackSpeed + attackSpeedLevel * 0.036f;
        
    }
   

    public override void FireAnim()
    {
        animator.SetTrigger("Attack");
        
        Invoke(nameof(Heal),0.3f);
        Invoke(nameof(DamageChaseTarget),0.3f);
        
    }

    void Heal()
    {
        var units = owner.ownerPlanet.battleUnits;
        
        if(units==null || units.Count==0)
            return;

        //要治疗的人的index

        BattleUnit targetUnit = null;
       
        float minPercent = 1;


        
        foreach (var unit in units)
        {
            if (unit.die == false)
            {
                var tmpPercent = (float) (unit.props.hp / unit.props.maxHp);
                if (tmpPercent <= minPercent)
                {
                    minPercent = tmpPercent;
                    targetUnit = unit;
                }
            }
        }
        
        if(targetUnit==null)
            return;

        targetUnit.OnAttacked(new AttackInfo(owner, AttackType.Heal, GetBaseAttackInfo().value));
        var shieldLevel = GetWeaponLevelByNbt("圣盾");
        if (GetWeaponLevelByNbt("圣盾")>0)
        {
            targetUnit.AddShield(Mathf.CeilToInt(shieldLevel*0.5f));
        }
        if (owner.IsTargetAlive())
        {
            ResFactory.Instance.CreateFx(GameConst.FX_BULLET_HIT, owner.chaseTarget.GetVictimPosition());
        }
        
        ResFactory.Instance.CreateFx(GameConst.FX_HEAL, targetUnit.transform.position);
    }
}
