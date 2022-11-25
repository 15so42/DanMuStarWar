using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBullet : ArrowBullet
{
    
    
    // public override int CalDamage()
    // {
    //     var levels = handWeapon.weaponNbt.enhancementLevels;
    //     var totalLevel = 0;
    //     for (int i = 0; i < levels.Count; i++)
    //     {
    //         totalLevel += levels[i].level;
    //     }
    //     var value = 5+totalLevel/3;
    //     return value;
    // }

    protected override void DamageVictim(IVictimAble victim)
    {
        //doNothing
    }

    protected override void DamageFx(IVictimAble victim)
    {
        base.DamageFx(victim);
        handWeapon.Damage();
    }
}
