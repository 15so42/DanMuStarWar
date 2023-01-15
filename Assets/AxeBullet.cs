using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBullet : ArrowBullet
{
    private void OnTriggerEnter(Collider other)
    {
        var victim = ValidCheck(other);
        if(victim==null)
            return;

        DamageVictim(victim);

        DamageFx(victim);
    }

    protected override void DamageVictim(IVictimAble victim)
    {
        //doNothing
    }

    protected override void DamageFx(IVictimAble victim)
    {
        base.DamageFx(victim);
        handWeapon.DamageOther(victim,handWeapon.GetBaseAttackInfo());
    }
}
