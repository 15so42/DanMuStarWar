using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Vampire : WarPlane
{
    public override void OnAttackOther(IVictimAble victimAble, AttackInfo attackInfo)
    {
        base.OnAttackOther(victimAble, attackInfo);
        OnAttacked(new AttackInfo(this,AttackType.Heal,1));
    }

   
}
